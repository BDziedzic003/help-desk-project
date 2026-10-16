using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Helpdesk.IntegrationTests.Support;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.IntegrationTests;

public class TicketsApiTests : IntegrationTestBase
{
    [Fact]
    public async Task CreateTicket_AuthenticatedUser_ReturnsCreatedOpenTicketOwnedByUser()
    {
        // Arrange
        var client = await CreateUserClientAsync("jan");

        // Act
        var ticket = await CreateTicketAsync(client, "Printer broken", TicketPriority.High);

        // Assert
        ticket.Status.Should().Be(TicketStatus.Open);
        ticket.Priority.Should().Be(TicketPriority.High);
        ticket.AuthorName.Should().Be("jan");
    }

    [Fact]
    public async Task CreateTicket_EmptyTitle_ReturnsBadRequest()
    {
        // Arrange
        var client = await CreateUserClientAsync("jan");
        var request = new TicketRequest { Title = "", Description = "Description" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tickets", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetTickets_RegularUser_ReturnsOnlyOwnTickets()
    {
        // Arrange
        var jan = await CreateUserClientAsync("jan");
        var anna = await CreateUserClientAsync("anna");
        await CreateTicketAsync(jan, "Jan ticket");
        await CreateTicketAsync(anna, "Anna ticket");

        // Act
        var tickets = await ReadAsync<List<TicketDto>>(await jan.GetAsync("/api/tickets"));

        // Assert
        tickets.Should().ContainSingle().Which.Title.Should().Be("Jan ticket");
    }

    [Fact]
    public async Task GetTicketById_TicketOfAnotherUser_ReturnsNotFound()
    {
        // Arrange
        var jan = await CreateUserClientAsync("jan");
        var anna = await CreateUserClientAsync("anna");
        var ticket = await CreateTicketAsync(anna);

        // Act
        var response = await jan.GetAsync($"/api/tickets/{ticket.Id}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetTickets_StatusFilterAndPrioritySort_ReturnsFilteredAndOrderedTickets()
    {
        // Arrange
        var admin = await CreateAdminClientAsync();
        var low = await CreateTicketAsync(admin, "Low", TicketPriority.Low);
        await CreateTicketAsync(admin, "High", TicketPriority.High);
        await CreateTicketAsync(admin, "Medium", TicketPriority.Medium);
        await admin.PutAsJsonAsync($"/api/tickets/{low.Id}/status", new ChangeStatusRequest { Status = TicketStatus.InProgress });

        // Act
        var open = await ReadAsync<List<TicketDto>>(
            await admin.GetAsync("/api/tickets?status=Open&sortBy=Priority&descending=true"));

        // Assert
        open.Select(t => t.Title).Should().Equal("High", "Medium");
    }

    [Fact]
    public async Task UpdateTicket_CompletedTicketAsRegularUser_ReturnsConflict()
    {
        // Arrange
        var jan = await CreateUserClientAsync("jan");
        var ticket = await CreateTicketAsync(jan);
        var admin = await CreateAdminClientAsync();
        await admin.PutAsJsonAsync($"/api/tickets/{ticket.Id}/status", new ChangeStatusRequest { Status = TicketStatus.Completed });
        var request = new TicketRequest { Title = "Changed", Description = "New text" };

        // Act
        var response = await jan.PutAsJsonAsync($"/api/tickets/{ticket.Id}", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task ChangeStatus_RegularUser_ReturnsForbidden()
    {
        // Arrange
        var jan = await CreateUserClientAsync("jan");
        var ticket = await CreateTicketAsync(jan);

        // Act
        var response = await jan.PutAsJsonAsync(
            $"/api/tickets/{ticket.Id}/status", new ChangeStatusRequest { Status = TicketStatus.Completed });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
