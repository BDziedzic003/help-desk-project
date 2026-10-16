using FluentAssertions;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Infrastructure;

namespace Helpdesk.UnitTests.Infrastructure;

public class UserDisplayExtensionsTests
{
    [Theory]
    [InlineData(Role.Admin, "Administrator")]
    [InlineData(Role.User, "Użytkownik")]
    public void ToDisplayName_Role_ReturnsPolishLabel(Role role, string expected)
    {
        // Arrange
        // (role provided by the test case)

        // Act
        var label = role.ToDisplayName();

        // Assert
        label.Should().Be(expected);
    }

    [Fact]
    public void ToBadgeClass_AdminRole_ReturnsDangerStyle()
    {
        // Arrange
        const Role role = Role.Admin;

        // Act
        var badgeClass = role.ToBadgeClass();

        // Assert
        badgeClass.Should().Be("bg-danger");
    }
}
