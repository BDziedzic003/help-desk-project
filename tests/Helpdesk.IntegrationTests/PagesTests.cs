using System.Net;
using FluentAssertions;
using Helpdesk.IntegrationTests.Support;

namespace Helpdesk.IntegrationTests;

public class PagesTests : IntegrationTestBase
{
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
}
