using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Admin.Users;

public class DeleteModel(IUserService userService) : PageModel
{
    public UserDto UserToDelete { get; private set; } = null!;

    public async Task<IActionResult> OnGetAsync(int id)
    {
        var result = await userService.GetByIdAsync(User.ToAuthenticatedUser(), id);
        if (!result.IsSuccess)
        {
            return NotFound();
        }

        UserToDelete = result.Value!;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(int id)
    {
        var result = await userService.DeleteAsync(User.ToAuthenticatedUser(), id);
        if (result.Error == ServiceError.NotFound)
        {
            return NotFound();
        }

        if (!result.IsSuccess)
        {
            return this.RedirectToPageWithError("Index", result.Error);
        }

        TempData[PageMessages.Success] = "Użytkownik został usunięty.";
        return RedirectToPage("Index");
    }
}
