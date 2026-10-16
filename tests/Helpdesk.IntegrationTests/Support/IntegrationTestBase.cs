using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using FluentAssertions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Helpdesk.IntegrationTests.Support;

/// <summary>
/// Gives every test its own application instance plus helpers for signing in and calling the API.
/// </summary>
public abstract partial class IntegrationTestBase : IDisposable
{
    protected const string UserPassword = "haslo123";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonStringEnumConverter() }
    };

    private readonly HelpdeskWebApplicationFactory _factory = new();

    protected HttpClient CreateAnonymousClient(bool followRedirects = true) =>
        _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = followRedirects });

    protected async Task<HttpClient> CreateAdminClientAsync() =>
        await CreateSignedInClientAsync(HelpdeskWebApplicationFactory.AdminUsername, HelpdeskWebApplicationFactory.AdminPassword);

    /// <summary>
    /// Creates a regular user through the admin API and returns a client signed in as that user.
    /// </summary>
    protected async Task<HttpClient> CreateUserClientAsync(string username)
    {
        var admin = await CreateAdminClientAsync();
        var response = await admin.PostAsJsonAsync("/api/users", new CreateUserRequest
        {
            Username = username,
            Password = UserPassword,
            Role = Role.User
        });
        response.EnsureSuccessStatusCode();

        return await CreateSignedInClientAsync(username, UserPassword);
    }

    protected async Task<HttpClient> CreateSignedInClientAsync(string username, string password)
    {
        var client = CreateAnonymousClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest { Username = username, Password = password });
        response.EnsureSuccessStatusCode();
        return client;
    }

    protected static async Task<TicketDto> CreateTicketAsync(
        HttpClient client, string title = "Printer broken", TicketPriority priority = TicketPriority.Medium)
    {
        var request = new TicketRequest { Title = title, Description = "Description", Priority = priority };
        var response = await client.PostAsJsonAsync("/api/tickets", request, JsonOptions);
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        return await ReadAsync<TicketDto>(response);
    }

    protected static async Task<T> ReadAsync<T>(HttpResponseMessage response)
    {
        var value = await response.Content.ReadFromJsonAsync<T>(JsonOptions);
        return value!;
    }

    protected static async Task<string> GetAntiforgeryTokenAsync(HttpClient client, string url)
    {
        var html = await client.GetStringAsync(url);
        return AntiforgeryTokenPattern().Match(html).Groups[1].Value;
    }

    public void Dispose()
    {
        _factory.Dispose();
        GC.SuppressFinalize(this);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"")]
    private static partial Regex AntiforgeryTokenPattern();
}
