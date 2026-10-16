using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services.Tickets;

public class TitleSortStrategy : ITicketSortStrategy
{
    public TicketSortField Field => TicketSortField.Title;

    public IOrderedQueryable<Ticket> Apply(IQueryable<Ticket> tickets, bool descending) =>
        descending ? tickets.OrderByDescending(ticket => ticket.Title) : tickets.OrderBy(ticket => ticket.Title);
}
