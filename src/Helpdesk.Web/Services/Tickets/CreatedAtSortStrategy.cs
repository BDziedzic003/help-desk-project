using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services.Tickets;

public class CreatedAtSortStrategy : ITicketSortStrategy
{
    public TicketSortField Field => TicketSortField.CreatedAt;

    public IOrderedQueryable<Ticket> Apply(IQueryable<Ticket> tickets, bool descending) =>
        descending ? tickets.OrderByDescending(ticket => ticket.CreatedAt) : tickets.OrderBy(ticket => ticket.CreatedAt);
}
