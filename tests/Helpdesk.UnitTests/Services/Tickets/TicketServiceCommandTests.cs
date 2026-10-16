using FluentAssertions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;

namespace Helpdesk.UnitTests.Services.Tickets;

public class TicketServiceCommandTests : TicketServiceTestBase
{
    private static TicketRequest NewRequest(string title = "New title") => new()
    {
        Title = title,
        Description = "New description",
        Priority = TicketPriority.High
    };

    [Fact]
    public async Task CreateAsync_ValidRequest_CreatesOpenTicketOwnedByActor()
    {
        // Arrange
        var request = NewRequest("  Padded title  ");

        // Act
        var ticket = await Service.CreateAsync(Owner, request);

        // Assert
        ticket.Title.Should().Be("Padded title");
        ticket.Status.Should().Be(TicketStatus.Open);
        ticket.UserId.Should().Be(Owner.Id);
        ticket.CreatedAt.Should().Be(StartTime.UtcDateTime);
        ticket.UpdatedAt.Should().Be(StartTime.UtcDateTime);
    }

    [Fact]
    public async Task UpdateAsync_TicketOfAnotherUser_ReturnsNotFound()
    {
        // Arrange
        var ticket = AddTicket(OtherUser);

        // Act
        var result = await Service.UpdateAsync(Owner, ticket.Id, NewRequest());

        // Assert
        result.Error.Should().Be(ServiceError.NotFound);
    }

    [Fact]
    public async Task UpdateAsync_CompletedTicketAsRegularUser_ReturnsLocked()
    {
        // Arrange
        var ticket = AddTicket(Owner, status: TicketStatus.Completed);

        // Act
        var result = await Service.UpdateAsync(Owner, ticket.Id, NewRequest());

        // Assert
        result.Error.Should().Be(ServiceError.Locked);
    }

    [Fact]
    public async Task UpdateAsync_CompletedTicketAsAdmin_UpdatesTicket()
    {
        // Arrange
        var ticket = AddTicket(Owner, status: TicketStatus.Completed);

        // Act
        var result = await Service.UpdateAsync(Admin, ticket.Id, NewRequest());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("New title");
        result.Value.Status.Should().Be(TicketStatus.Completed);
    }

    [Fact]
    public async Task ChangeStatusAsync_RegularUser_ReturnsForbidden()
    {
        // Arrange
        var ticket = AddTicket(Owner);
        var request = new ChangeStatusRequest { Status = TicketStatus.Completed };

        // Act
        var result = await Service.ChangeStatusAsync(Owner, ticket.Id, request);

        // Assert
        result.Error.Should().Be(ServiceError.Forbidden);
    }
}
