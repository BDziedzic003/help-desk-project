using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services.Tickets;

public class PriorityTicketFilter : ITicketFilter
{
    public IQueryable<Ticket> Apply(IQueryable<Ticket> tickets, TicketQuery query) =>
        query.Priority is { } priority ? tickets.Where(ticket => ticket.Priority == priority) : tickets;
}
