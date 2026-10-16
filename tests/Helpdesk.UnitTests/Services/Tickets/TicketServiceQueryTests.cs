using FluentAssertions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;

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
    public async Task GetAllAsync_StatusFilter_ReturnsOnlyMatchingStatus()
    {
        // Arrange
        AddTicket(Owner, "Open one", status: TicketStatus.Open);
        AddTicket(Owner, "Done one", status: TicketStatus.Completed);

        // Act
        var tickets = await Service.GetAllAsync(Owner, new TicketQuery { Status = TicketStatus.Completed });

        // Assert
        tickets.Should().ContainSingle().Which.Title.Should().Be("Done one");
    }

    [Fact]
    public async Task GetAllAsync_PriorityFilter_ReturnsOnlyMatchingPriority()
    {
        // Arrange
        AddTicket(Owner, "Urgent", TicketPriority.High);
        AddTicket(Owner, "Whenever", TicketPriority.Low);

        // Act
        var tickets = await Service.GetAllAsync(Owner, new TicketQuery { Priority = TicketPriority.High });

        // Assert
        tickets.Should().ContainSingle().Which.Title.Should().Be("Urgent");
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
    public async Task GetAllAsync_SearchWithLikeWildcard_TreatsWildcardAsLiteral()
    {
        // Arrange
        AddTicket(Owner, "Disk 100% full");
        AddTicket(Owner, "Disk is fine");

        // Act
        var tickets = await Service.GetAllAsync(Owner, new TicketQuery { Search = "100%" });

        // Assert
        tickets.Should().ContainSingle().Which.Title.Should().Be("Disk 100% full");
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

    [Fact]
    public async Task GetAllAsync_SortByTitleAscending_ReturnsAlphabeticalOrder()
    {
        // Arrange
        AddTicket(Owner, "Charlie");
        AddTicket(Owner, "Alpha");
        AddTicket(Owner, "Bravo");
        var query = new TicketQuery { SortBy = TicketSortField.Title, Descending = false };

        // Act
        var tickets = await Service.GetAllAsync(Owner, query);

        // Assert
        tickets.Select(t => t.Title).Should().Equal("Alpha", "Bravo", "Charlie");
    }

    [Fact]
    public async Task GetAllAsync_DefaultQuery_ReturnsNewestFirst()
    {
        // Arrange
        AddTicket(Owner, "Old", createdAt: StartTime.UtcDateTime.AddDays(-2));
        AddTicket(Owner, "New", createdAt: StartTime.UtcDateTime);

        // Act
        var tickets = await Service.GetAllAsync(Owner, new TicketQuery());

        // Assert
        tickets.Select(t => t.Title).Should().Equal("New", "Old");
    }

    [Fact]
    public async Task GetByIdAsync_TicketOfAnotherUser_ReturnsNotFound()
    {
        // Arrange
        var ticket = AddTicket(OtherUser);

        // Act
        var result = await Service.GetByIdAsync(Owner, ticket.Id);

        // Assert
        result.Error.Should().Be(ServiceError.NotFound);
    }

    [Fact]
    public async Task GetByIdAsync_OwnTicket_ReturnsTicketWithAuthorName()
    {
        // Arrange
        var ticket = AddTicket(Owner, "Mine");

        // Act
        var result = await Service.GetByIdAsync(Owner, ticket.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("Mine");
        result.Value.AuthorName.Should().Be("owner");
    }
}
