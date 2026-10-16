using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Admin.Users;

public class CreateModel(IUserService userService) : PageModel
{
    [BindProperty]
    public CreateUserRequest Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await userService.CreateAsync(User.ToAuthenticatedUser(), Input);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, ServiceErrorMessages.For(result.Error));
            return Page();
        }

        TempData[PageMessages.Success] = "Użytkownik został utworzony.";
        return RedirectToPage("Index");
    }
}
