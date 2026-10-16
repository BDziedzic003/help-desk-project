using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Tickets;

public class DeleteModel(ITicketService ticketService) : PageModel
{
    public TicketDto Ticket { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var actor = User.ToAuthenticatedUser();
        var result = await ticketService.GetByIdAsync(actor, id);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        Ticket = result.Value!;
        if (TicketRules.IsLockedFor(actor.Role, Ticket.Status))
        {
            return this.RedirectToPageWithError("Index", ServiceError.Locked);
        }

        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var result = await ticketService.DeleteAsync(User.ToAuthenticatedUser(), id);
        if (result.Error == ServiceError.NotFound)
        {
            return NotFound();
        }

        if (!result.IsSuccess)
        {
            return this.RedirectToPageWithError("Index", result.Error);
        }

        TempData[PageMessages.Success] = "Zgłoszenie zostało usunięte.";
        return RedirectToPage("Index");
    }
}
