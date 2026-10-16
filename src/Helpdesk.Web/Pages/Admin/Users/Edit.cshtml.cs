using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Admin.Users;

public class EditModel(IUserService userService) : PageModel
{
    [BindProperty]
    public UpdateUserRequest Input { get; set; } = new();

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await userService.GetByIdAsync(User.ToAuthenticatedUser(), id);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        Input = new UpdateUserRequest { Username = result.Value!.Username, Role = result.Value.Role };
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await userService.UpdateAsync(User.ToAuthenticatedUser(), id, Input);
        if (result.Error == ServiceError.NotFound)
        {
            return NotFound();
        }

        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, ServiceErrorMessages.For(result.Error));
            return Page();
        }

        TempData[PageMessages.Success] = "Użytkownik został zaktualizowany.";
        return RedirectToPage("Index");
    }
}
