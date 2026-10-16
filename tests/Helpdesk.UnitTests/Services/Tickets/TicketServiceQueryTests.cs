using FluentAssertions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.UnitTests.Services.Tickets;

public class TicketServiceQueryTests : TicketServiceTestBase
{
    [Fact]
    public async Task GetAllAsync_RegularUser_ReturnsOnlyOwnTickets()
    {
        // Arrange
        AddTicket(Owner, "Mine");
        AddTicket(OtherUser, "Not mine");

        // Act
        var tickets = await Service.GetAllAsync(Owner, new TicketQuery());

        // Assert
        tickets.Should().ContainSingle().Which.Title.Should().Be("Mine");
    }

    [Fact]
    public async Task GetAllAsync_Admin_ReturnsTicketsOfAllUsers()
    {
        // Arrange
        AddTicket(Owner);
        AddTicket(OtherUser);

        // Act
        var tickets = await Service.GetAllAsync(Admin, new TicketQuery());

        // Assert
        tickets.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAllAsync_TitleSearch_MatchesCaseInsensitiveSubstring()
    {
        // Arrange
        AddTicket(Owner, "Printer is broken");
        AddTicket(Owner, "Cannot login");

        // Act
        var tickets = await Service.GetAllAsync(Owner, new TicketQuery { Search = "PRINTER" });

        // Assert
        tickets.Should().ContainSingle().Which.Title.Should().Be("Printer is broken");
    }

    [Fact]
    public async Task GetAllAsync_SortByPriorityDescending_ReturnsHighMediumLow()
    {
        // Arrange
        AddTicket(Owner, "Medium", TicketPriority.Medium);
        AddTicket(Owner, "Low", TicketPriority.Low);
        AddTicket(Owner, "High", TicketPriority.High);
        var query = new TicketQuery { SortBy = TicketSortField.Priority, Descending = true };

        // Act
        var tickets = await Service.GetAllAsync(Owner, query);

        // Assert
        tickets.Select(t => t.Title).Should().Equal("High", "Medium", "Low");
    }
}
