using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Tickets;

public class EditModel(ITicketService ticketService) : PageModel
{
    [BindProperty]
    public TicketRequest Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var actor = User.ToAuthenticatedUser();
        var result = await ticketService.GetByIdAsync(actor, id);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        var ticket = result.Value!;
        if (TicketRules.IsLockedFor(actor.Role, ticket.Status))
        {
            return this.RedirectToPageWithError("Index", ServiceError.Locked);
        }

        Input = new TicketRequest { Title = ticket.Title, Description = ticket.Description, Priority = ticket.Priority };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await ticketService.UpdateAsync(User.ToAuthenticatedUser(), id, Input);
        if (result.Error == ServiceError.NotFound)
        {
            return NotFound();
        }

        if (!result.IsSuccess)
        {
            return this.RedirectToPageWithError("Index", result.Error);
        }

        TempData[PageMessages.Success] = "Zgłoszenie zostało zaktualizowane.";
        return RedirectToPage("Index");
    }
}
