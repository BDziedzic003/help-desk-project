using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Helpdesk.IntegrationTests.Support;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.IntegrationTests;

public class AuthApiTests : IntegrationTestBase
{
    [Fact]
    public async Task Login_SeededAdminCredentials_ReturnsAdminUser()
    {
        // Arrange
        var client = CreateAnonymousClient();
        var request = new LoginRequest { Username = "admin", Password = "admin" };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var user = await ReadAsync<AuthenticatedUser>(response);
        user.Username.Should().Be("admin");
        user.Role.Should().Be(Role.Admin);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateAnonymousClient();
        var request = new LoginRequest { Username = "admin", Password = "wrong" };

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetTickets_Anonymous_ReturnsUnauthorized()
    {
        // Arrange
        var client = CreateAnonymousClient();

        // Act
        var response = await client.GetAsync("/api/tickets");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
