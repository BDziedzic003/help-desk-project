using FluentAssertions;
using Helpdesk.Web.Services;

namespace Helpdesk.UnitTests.Services;

public class Pbkdf2PasswordHasherTests
{
    private readonly Pbkdf2PasswordHasher _hasher = new();

    [Fact]
    public void Hash_ValidPassword_DoesNotReturnPlainText()
    {
        // Arrange
        const string password = "Secret123!";

        // Act
        var hash = _hasher.Hash(password);

        // Assert
        hash.Should().NotBeNullOrEmpty().And.NotContain(password);
    }

    [Fact]
    public void Hash_SamePasswordTwice_ReturnsDifferentHashes()
    {
        // Arrange
        const string password = "Secret123!";

        // Act
        var first = _hasher.Hash(password);
        var second = _hasher.Hash(password);

        // Assert
        first.Should().NotBe(second);
    }

    [Fact]
    public void Verify_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var hash = _hasher.Hash("Secret123!");

        // Act
        var result = _hasher.Verify("Secret123!", hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void Verify_WrongPassword_ReturnsFalse()
    {
        // Arrange
        var hash = _hasher.Hash("Secret123!");

        // Act
        var result = _hasher.Verify("WrongPassword", hash);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void Verify_MalformedHash_ReturnsFalse()
    {
        // Arrange
        const string malformedHash = "not-a-valid-hash";

        // Act
        var result = _hasher.Verify("Secret123!", malformedHash);

        // Assert
        result.Should().BeFalse();
    }
}
