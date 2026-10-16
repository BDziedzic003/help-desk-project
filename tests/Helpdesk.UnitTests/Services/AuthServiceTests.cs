using FluentAssertions;
using Helpdesk.UnitTests.Support;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;

namespace Helpdesk.UnitTests.Services;

public class AuthServiceTests : IDisposable
{
    private const string Username = "jan";
    private const string Password = "Secret123!";

    private readonly TestDatabase _database = new();
    private readonly Pbkdf2PasswordHasher _hasher = new();
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _service = new AuthService(_database.Context, _hasher);
        _database.Context.Users.Add(new User { Username = Username, PasswordHash = _hasher.Hash(Password), Role = Role.User });
        _database.Context.SaveChanges();
    }

    [Fact]
    public async Task AuthenticateAsync_ValidCredentials_ReturnsSuccessWithUser()
    {
        // Arrange
        // (user seeded in the constructor)

        // Act
        var result = await _service.AuthenticateAsync(Username, Password);

        // Assert
        result.Succeeded.Should().BeTrue();
        result.User.Should().NotBeNull();
        result.User!.Username.Should().Be(Username);
        result.User.Role.Should().Be(Role.User);
    }

    [Fact]
    public async Task AuthenticateAsync_WrongPassword_ReturnsFailure()
    {
        // Arrange
        const string wrongPassword = "WrongPassword";

        // Act
        var result = await _service.AuthenticateAsync(Username, wrongPassword);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.User.Should().BeNull();
        result.Error.Should().Be(AuthService.InvalidCredentialsError);
    }

    [Fact]
    public async Task AuthenticateAsync_UnknownUser_ReturnsFailure()
    {
        // Arrange
        const string unknownUsername = "nobody";

        // Act
        var result = await _service.AuthenticateAsync(unknownUsername, Password);

        // Assert
        result.Succeeded.Should().BeFalse();
        result.Error.Should().Be(AuthService.InvalidCredentialsError);
    }

    [Fact]
    public async Task AuthenticateAsync_AdminUser_ReturnsAdminRole()
    {
        // Arrange
        _database.Context.Users.Add(new User { Username = "root", PasswordHash = _hasher.Hash(Password), Role = Role.Admin });
        await _database.Context.SaveChangesAsync();

        // Act
        var result = await _service.AuthenticateAsync("root", Password);

        // Assert
        result.User!.Role.Should().Be(Role.Admin);
    }

    public void Dispose() => _database.Dispose();
}
