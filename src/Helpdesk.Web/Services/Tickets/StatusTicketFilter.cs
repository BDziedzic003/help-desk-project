using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services.Tickets;

public class StatusTicketFilter : ITicketFilter
{
    public IQueryable<Ticket> Apply(IQueryable<Ticket> tickets, TicketQuery query) =>
        query.Status is { } status ? tickets.Where(ticket => ticket.Status == status) : tickets;
}
