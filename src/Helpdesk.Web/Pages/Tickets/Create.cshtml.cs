using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Tickets;

public class CreateModel(ITicketService ticketService) : PageModel
{
    [BindProperty]
    public TicketRequest Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        await ticketService.CreateAsync(User.ToAuthenticatedUser(), Input);
        TempData[PageMessages.Success] = "Zgłoszenie zostało utworzone.";

        return RedirectToPage("Index");
    }
}
