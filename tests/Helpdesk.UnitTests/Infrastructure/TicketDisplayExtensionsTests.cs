using FluentAssertions;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Infrastructure;

namespace Helpdesk.UnitTests.Infrastructure;

public class TicketDisplayExtensionsTests
{
    [Theory]
    [InlineData(TicketStatus.Open, "Otwarte")]
    [InlineData(TicketStatus.InProgress, "W trakcie")]
    [InlineData(TicketStatus.Completed, "Zakończone")]
    public void ToDisplayName_TicketStatus_ReturnsPolishLabel(TicketStatus status, string expected)
    {
        // Arrange
        // (status provided by the test case)

        // Act
        var label = status.ToDisplayName();

        // Assert
        label.Should().Be(expected);
    }

    [Theory]
    [InlineData(TicketPriority.Low, "Niski")]
    [InlineData(TicketPriority.Medium, "Średni")]
    [InlineData(TicketPriority.High, "Wysoki")]
    public void ToDisplayName_TicketPriority_ReturnsPolishLabel(TicketPriority priority, string expected)
    {
        // Arrange
        // (priority provided by the test case)

        // Act
        var label = priority.ToDisplayName();

        // Assert
        label.Should().Be(expected);
    }

    [Fact]
    public void ToDisplayString_UtcDate_ReturnsLocalTimeInExpectedFormat()
    {
        // Arrange
        var utcDate = new DateTime(2026, 3, 5, 10, 30, 0, DateTimeKind.Unspecified);
        var expected = DateTime.SpecifyKind(utcDate, DateTimeKind.Utc).ToLocalTime().ToString("yyyy-MM-dd HH:mm");

        // Act
        var text = utcDate.ToDisplayString();

        // Assert
        text.Should().Be(expected);
    }
}
