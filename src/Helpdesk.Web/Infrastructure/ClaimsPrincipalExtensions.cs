using System.Security.Claims;
using Helpdesk.Web.Contracts;
using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Infrastructure;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Rebuilds the signed-in user from the claims written by <see cref="ClaimsPrincipalFactory"/>.
    /// </summary>
    public static AuthenticatedUser ToAuthenticatedUser(this ClaimsPrincipal principal)
    {
        var id = int.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var username = principal.FindFirstValue(ClaimTypes.Name)!;
        var role = Enum.Parse<Role>(principal.FindFirstValue(ClaimTypes.Role)!);

        return new AuthenticatedUser(id, username, role);
    }
}
