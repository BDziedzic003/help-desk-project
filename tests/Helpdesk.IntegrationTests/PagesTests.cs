using System.Net;
using FluentAssertions;
using Helpdesk.IntegrationTests.Support;

namespace Helpdesk.IntegrationTests;

public class PagesTests : IntegrationTestBase
{
    [Fact]
    public async Task GetLoginPage_Anonymous_ReturnsPolishLoginForm()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/Account/Login");
        var html = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        html.Should().Contain("Logowanie").And.Contain("type=\"password\"");
    }

    [Fact]
    public async Task GetTicketsPage_Anonymous_RedirectsToLogin()
    {
        // Arrange
        var client = CreateAnonymousClient(followRedirects: false);

        // Act
        var response = await client.GetAsync("/Tickets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Redirect);
        response.Headers.Location!.AbsolutePath.Should().Be("/Account/Login");
    }

    [Fact]
    public async Task PostLoginPage_ValidCredentials_RedirectsAndOpensTicketsPage()
    {
        // Arrange
        var client = CreateAnonymousClient(followRedirects: false);
        var token = await GetAntiforgeryTokenAsync(client, "/Account/Login");
        var form = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Input.Username"] = "admin",
            ["Input.Password"] = "admin",
            ["__RequestVerificationToken"] = token
        });

        // Act
        var loginResponse = await client.PostAsync("/Account/Login", form);
        var ticketsResponse = await client.GetAsync("/Tickets");

        // Assert
        loginResponse.StatusCode.Should().Be(HttpStatusCode.Redirect);
        ticketsResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAdminUsersPage_RegularUser_IsDenied()
    {
        // Arrange
        var jan = await CreateUserClientAsync("jan");

        // Act
        var response = await jan.GetAsync("/Admin/Users");

        // Assert
        response.RequestMessage!.RequestUri!.AbsolutePath.Should().Be("/Account/AccessDenied");
    }

    [Fact]
    public async Task GetAdminUsersPage_Admin_ListsUsers()
    {
        // Arrange
        var admin = await CreateAdminClientAsync();

        // Act
        var response = await admin.GetAsync("/Admin/Users");
        var html = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        html.Should().Contain("Użytkownicy").And.Contain("admin");
    }
}
