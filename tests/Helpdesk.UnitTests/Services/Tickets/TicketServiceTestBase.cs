using Helpdesk.UnitTests.Support;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Data;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;
using Helpdesk.Web.Services.Tickets;

namespace Helpdesk.UnitTests.Services.Tickets;

/// <summary>
/// Shared setup: a real SQLite schema, three seeded users and a service wired with the production filters and sorters.
/// </summary>
public abstract class TicketServiceTestBase : IDisposable
{
    protected static readonly DateTimeOffset StartTime = new(2026, 1, 10, 12, 0, 0, TimeSpan.Zero);

    private readonly TestDatabase _database = new();

    protected TicketServiceTestBase()
    {
        Clock = new FakeTimeProvider(StartTime);
        Service = new TicketService(
            Context,
            [new StatusTicketFilter(), new PriorityTicketFilter(), new TitleSearchTicketFilter()],
            [new CreatedAtSortStrategy(), new PrioritySortStrategy(), new TitleSortStrategy()],
            Clock);

        Owner = AddUser("owner", Role.User);
        OtherUser = AddUser("other", Role.User);
        Admin = AddUser("admin", Role.Admin);
    }

    protected HelpdeskDbContext Context => _database.Context;

    protected FakeTimeProvider Clock { get; }

    protected TicketService Service { get; }

    protected AuthenticatedUser Owner { get; }

    protected AuthenticatedUser OtherUser { get; }

    protected AuthenticatedUser Admin { get; }

    protected Ticket AddTicket(
        AuthenticatedUser author,
        string title = "Printer broken",
        TicketPriority priority = TicketPriority.Medium,
        TicketStatus status = TicketStatus.Open,
        DateTime? createdAt = null)
    {
        var created = createdAt ?? StartTime.UtcDateTime;
        var ticket = new Ticket
        {
            Title = title,
            Description = "Description",
            Priority = priority,
            Status = status,
            CreatedAt = created,
            UpdatedAt = created,
            UserId = author.Id
        };
        Context.Tickets.Add(ticket);
        Context.SaveChanges();
        return ticket;
    }

    public void Dispose() => _database.Dispose();

    private AuthenticatedUser AddUser(string username, Role role)
    {
        var user = new User { Username = username, PasswordHash = "hash", Role = role };
        Context.Users.Add(user);
        Context.SaveChanges();
        return new AuthenticatedUser(user.Id, user.Username, user.Role);
    }
}
