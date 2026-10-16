using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Helpdesk.IntegrationTests.Support;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.IntegrationTests;

public class UsersApiTests : IntegrationTestBase
{
    [Fact]
    public async Task GetUsers_RegularUser_ReturnsForbidden()
    {
        // Arrange
        var jan = await CreateUserClientAsync("jan");

        // Act
        var response = await jan.GetAsync("/api/users");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task CreateUser_Admin_NewUserCanSignIn()
    {
        // Arrange
        var admin = await CreateAdminClientAsync();
        var request = new CreateUserRequest { Username = "anna", Password = "haslo123", Role = Role.User };

        // Act
        var response = await admin.PostAsJsonAsync("/api/users", request);
        var loginResponse = await CreateAnonymousClient()
            .PostAsJsonAsync("/api/auth/login", new LoginRequest { Username = "anna", Password = "haslo123" });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task CreateUser_DuplicateUsername_ReturnsConflict()
    {
        // Arrange
        var admin = await CreateAdminClientAsync();
        var request = new CreateUserRequest { Username = "ADMIN", Password = "haslo123" };

        // Act
        var response = await admin.PostAsJsonAsync("/api/users", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteUser_OwnAdminAccount_ReturnsConflict()
    {
        // Arrange
        var admin = await CreateAdminClientAsync();
        var users = await ReadAsync<List<UserDto>>(await admin.GetAsync("/api/users"));
        var adminId = users.Single(u => u.Username == "admin").Id;

        // Act
        var response = await admin.DeleteAsync($"/api/users/{adminId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task DeleteUser_UserWithTickets_RemovesUserAndTickets()
    {
        // Arrange
        var jan = await CreateUserClientAsync("jan");
        await CreateTicketAsync(jan);
        var admin = await CreateAdminClientAsync();
        var users = await ReadAsync<List<UserDto>>(await admin.GetAsync("/api/users"));
        var janId = users.Single(u => u.Username == "jan").Id;

        // Act
        var response = await admin.DeleteAsync($"/api/users/{janId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var tickets = await ReadAsync<List<TicketDto>>(await admin.GetAsync("/api/tickets"));
        tickets.Should().BeEmpty();
    }
}
