using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Services.Tickets;

/// <summary>
/// A single, independent filtering rule. All registered filters are applied one after another.
/// </summary>
public interface ITicketFilter
{
    IQueryable<Ticket> Apply(IQueryable<Ticket> tickets, TicketQuery query);
}
