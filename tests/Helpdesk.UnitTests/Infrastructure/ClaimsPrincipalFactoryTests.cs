using System.Security.Claims;
using FluentAssertions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Infrastructure;

namespace Helpdesk.UnitTests.Infrastructure;

public class ClaimsPrincipalFactoryTests
{
    [Fact]
    public void Create_AdminUser_ContainsIdNameAndRoleClaims()
    {
        // Arrange
        var user = new AuthenticatedUser(7, "root", Role.Admin);

        // Act
        var principal = ClaimsPrincipalFactory.Create(user);

        // Assert
        principal.FindFirst(ClaimTypes.NameIdentifier)!.Value.Should().Be("7");
        principal.Identity!.Name.Should().Be("root");
        principal.IsInRole(nameof(Role.Admin)).Should().BeTrue();
        principal.Identity.IsAuthenticated.Should().BeTrue();
    }
}
