using FluentAssertions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;
using Microsoft.EntityFrameworkCore;

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
    public async Task UpdateAsync_OwnOpenTicket_UpdatesFieldsAndUpdatedAt()
    {
        // Arrange
        var ticket = AddTicket(Owner);
        Clock.Now = StartTime.AddHours(3);

        // Act
        var result = await Service.UpdateAsync(Owner, ticket.Id, NewRequest());

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be("New title");
        result.Value.Priority.Should().Be(TicketPriority.High);
        result.Value.UpdatedAt.Should().Be(Clock.Now.UtcDateTime);
        result.Value.CreatedAt.Should().Be(StartTime.UtcDateTime);
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
    public async Task DeleteAsync_OwnOpenTicket_RemovesTicket()
    {
        // Arrange
        var ticket = AddTicket(Owner);

        // Act
        var result = await Service.DeleteAsync(Owner, ticket.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await Context.Tickets.AnyAsync(t => t.Id == ticket.Id)).Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_CompletedTicketAsRegularUser_ReturnsLockedAndKeepsTicket()
    {
        // Arrange
        var ticket = AddTicket(Owner, status: TicketStatus.Completed);

        // Act
        var result = await Service.DeleteAsync(Owner, ticket.Id);

        // Assert
        result.Error.Should().Be(ServiceError.Locked);
        (await Context.Tickets.AnyAsync(t => t.Id == ticket.Id)).Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_TicketOfAnotherUserAsAdmin_RemovesTicket()
    {
        // Arrange
        var ticket = AddTicket(OtherUser, status: TicketStatus.Completed);

        // Act
        var result = await Service.DeleteAsync(Admin, ticket.Id);

        // Assert
        result.IsSuccess.Should().BeTrue();
        (await Context.Tickets.AnyAsync(t => t.Id == ticket.Id)).Should().BeFalse();
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

    [Fact]
    public async Task ChangeStatusAsync_Admin_ChangesStatusAndUpdatedAt()
    {
        // Arrange
        var ticket = AddTicket(Owner);
        Clock.Now = StartTime.AddDays(1);
        var request = new ChangeStatusRequest { Status = TicketStatus.InProgress };

        // Act
        var result = await Service.ChangeStatusAsync(Admin, ticket.Id, request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Status.Should().Be(TicketStatus.InProgress);
        result.Value.UpdatedAt.Should().Be(Clock.Now.UtcDateTime);
    }

    [Fact]
    public async Task ChangeStatusAsync_UnknownTicket_ReturnsNotFound()
    {
        // Arrange
        var request = new ChangeStatusRequest { Status = TicketStatus.Completed };

        // Act
        var result = await Service.ChangeStatusAsync(Admin, 9999, request);

        // Assert
        result.Error.Should().Be(ServiceError.NotFound);
    }
}
