using FluentAssertions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Infrastructure;

namespace Helpdesk.UnitTests.Infrastructure;

public class ClaimsPrincipalExtensionsTests
{
    [Fact]
    public void ToAuthenticatedUser_PrincipalFromFactory_RestoresOriginalUser()
    {
        // Arrange
        var user = new AuthenticatedUser(42, "root", Role.Admin);
        var principal = ClaimsPrincipalFactory.Create(user);

        // Act
        var restored = principal.ToAuthenticatedUser();

        // Assert
        restored.Should().Be(user);
    }
}
