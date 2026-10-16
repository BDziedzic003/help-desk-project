using Helpdesk.Web.Contracts;
using Helpdesk.Web.Infrastructure;
using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Pages.Admin.Users;

public class IndexModel(IUserService userService) : PageModel
{
    public IReadOnlyList<UserDto> Users { get; private set; } = [];

    public int CurrentUserId { get; private set; }

    public async Task OnGetAsync()
    {
        var actor = User.ToAuthenticatedUser();
        CurrentUserId = actor.Id;

        var result = await userService.GetAllAsync(actor);
        Users = result.Value ?? [];
    }
}
