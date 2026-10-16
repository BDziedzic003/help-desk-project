using FluentAssertions;
using Helpdesk.UnitTests.Support;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.UnitTests.Services;

public class UserServiceTests : IDisposable
{
    private const string Password = "Secret123!";

    private readonly TestDatabase _database = new();
    private readonly Pbkdf2PasswordHasher _hasher = new();
    private readonly UserService _service;
    private readonly AuthenticatedUser _admin;
    private readonly AuthenticatedUser _regularUser;

    public UserServiceTests()
    {
        _service = new UserService(_database.Context, _hasher);
        _admin = AddUser("admin", Role.Admin);
        _regularUser = AddUser("jan", Role.User);
    }

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesUserWithHashedPassword()
    {
        // Arrange
        var request = new CreateUserRequest { Username = "  anna  ", Password = Password, Role = Role.User };

        // Act
        var result = await _service.CreateAsync(_admin, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        var stored = await _database.Context.Users.SingleAsync(u => u.Username == "anna");
        stored.PasswordHash.Should().NotBe(Password);
        _hasher.Verify(Password, stored.PasswordHash).Should().BeTrue();
        stored.Role.Should().Be(Role.User);
    }

    [Fact]
    public async Task CreateAsync_UsernameDifferingOnlyByCase_ReturnsUsernameTaken()
    {
        // Arrange
        var request = new CreateUserRequest { Username = "JAN", Password = Password };

        // Act
        var result = await _service.CreateAsync(_admin, request);

        // Assert
        result.Error.Should().Be(ServiceError.UsernameTaken);
    }

    [Fact]
    public async Task UpdateAsync_DemotingLastAdmin_ReturnsLastAdminProtected()
    {
        // Arrange
        var request = new UpdateUserRequest { Username = "admin", Role = Role.User };

        // Act
        var result = await _service.UpdateAsync(_admin, _admin.Id, request);

        // Assert
        result.Error.Should().Be(ServiceError.LastAdminProtected);
    }

    [Fact]
    public async Task DeleteAsync_RegularUserWithTickets_RemovesUserAndTickets()
    {
        // Arrange
        _database.Context.Tickets.Add(NewTicket(_regularUser.Id));
        await _database.Context.SaveChangesAsync();

        // Act
        var result = await _service.DeleteAsync(_admin, _regularUser.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await _database.Context.Users.AnyAsync(u => u.Id == _regularUser.Id)).Should().BeFalse();
        (await _database.Context.Tickets.AnyAsync()).Should().BeFalse();
    }

    public void Dispose() => _database.Dispose();

    private AuthenticatedUser AddUser(string username, Role role)
    {
        var user = new User { Username = username, PasswordHash = _hasher.Hash(Password), Role = role };
        _database.Context.Users.Add(user);
        _database.Context.SaveChanges();
        return new AuthenticatedUser(user.Id, user.Username, user.Role);
    }

    private static Ticket NewTicket(int userId) => new()
    {
        Title = "Ticket",
        Description = "Description",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        UserId = userId
    };
}
