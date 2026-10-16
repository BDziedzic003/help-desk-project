using FluentAssertions;
using Helpdesk.UnitTests.Support;
using Helpdesk.Web.Data;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Helpdesk.UnitTests.Data;

public class DbSeederTests : IDisposable
{
    private readonly TestDatabase _database = new();
    private readonly Pbkdf2PasswordHasher _hasher = new();
    private readonly DbSeeder _seeder;

    public DbSeederTests()
    {
        var options = Options.Create(new AdminSeedOptions { Username = "admin", Password = "Admin123!" });
        _seeder = new DbSeeder(_database.Context, _hasher, options);
    }

    [Fact]
    public async Task SeedAsync_EmptyDatabase_CreatesAdminWithHashedPassword()
    {
        // Arrange
        // (empty database)

        // Act
        await _seeder.SeedAsync();

        // Assert
        var admin = await _database.Context.Users.SingleAsync();
        admin.Username.Should().Be("admin");
        admin.Role.Should().Be(Role.Admin);
        admin.PasswordHash.Should().NotBe("Admin123!");
        _hasher.Verify("Admin123!", admin.PasswordHash).Should().BeTrue();
    }

    [Fact]
    public async Task SeedAsync_CalledTwice_DoesNotDuplicateAdmin()
    {
        // Arrange
        await _seeder.SeedAsync();

        // Act
        await _seeder.SeedAsync();

        // Assert
        (await _database.Context.Users.CountAsync()).Should().Be(1);
    }

    public void Dispose() => _database.Dispose();
}
