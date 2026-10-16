using System.Linq.Expressions;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services.Tickets;

/// <summary>
/// Priorities are stored as text, so they are ranked explicitly instead of being sorted alphabetically.
/// </summary>
public class PrioritySortStrategy : ITicketSortStrategy
{
    private static readonly Expression<Func<Ticket, int>> Rank = ticket =>
        ticket.Priority == TicketPriority.High ? 2 : ticket.Priority == TicketPriority.Medium ? 1 : 0;

    public TicketSortField Field => TicketSortField.Priority;

    public IOrderedQueryable<Ticket> Apply(IQueryable<Ticket> tickets, bool descending) =>
        descending ? tickets.OrderByDescending(Rank) : tickets.OrderBy(Rank);
}
