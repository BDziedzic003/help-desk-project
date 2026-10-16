using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services.Tickets;

/// <summary>
/// Sorting strategy for one <see cref="TicketSortField"/>.
/// </summary>
public interface ITicketSortStrategy
{
    TicketSortField Field { get; }

    IOrderedQueryable<Ticket> Apply(IQueryable<Ticket> tickets, bool descending);
}
