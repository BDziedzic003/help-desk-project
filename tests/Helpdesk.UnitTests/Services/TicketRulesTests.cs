using FluentAssertions;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;

namespace Helpdesk.UnitTests.Services;

public class TicketRulesTests
{
    [Theory]
    [InlineData(TicketStatus.Open)]
    [InlineData(TicketStatus.InProgress)]
    public void IsLockedFor_RegularUserAndUnfinishedTicket_ReturnsFalse(TicketStatus status)
    {
        // Arrange
        const Role role = Role.User;

        // Act
        var isLocked = TicketRules.IsLockedFor(role, status);

        // Assert
        isLocked.Should().BeFalse();
    }

    [Fact]
    public void IsLockedFor_RegularUserAndCompletedTicket_ReturnsTrue()
    {
        // Arrange
        const Role role = Role.User;

        // Act
        var isLocked = TicketRules.IsLockedFor(role, TicketStatus.Completed);

        // Assert
        isLocked.Should().BeTrue();
    }

    [Fact]
    public void IsLockedFor_AdminAndCompletedTicket_ReturnsFalse()
    {
        // Arrange
        const Role role = Role.Admin;

        // Act
        var isLocked = TicketRules.IsLockedFor(role, TicketStatus.Completed);

        // Assert
        isLocked.Should().BeFalse();
    }
}
