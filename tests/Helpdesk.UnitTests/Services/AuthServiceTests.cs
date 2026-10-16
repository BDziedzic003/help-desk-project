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

    public void Dispose() => _database.Dispose();
}
