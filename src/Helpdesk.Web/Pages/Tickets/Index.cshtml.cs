using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Tickets;

public class IndexModel(ITicketService ticketService) : PageModel
{
    public TicketQuery Query { get; private set; } = new();

    public IReadOnlyList<TicketDto> Tickets { get; private set; } = [];

    public bool IsAdmin => User.IsInRole(nameof(Role.Admin));

    public async Task OnGetAsync(TicketQuery query)
    {
        Query = query;
        Tickets = await ticketService.GetAllAsync(User.ToAuthenticatedUser(), query);
    }

    public bool CanModify(TicketDto ticket) =>
        !TicketRules.IsLockedFor(User.ToAuthenticatedUser().Role, ticket.Status);
}
