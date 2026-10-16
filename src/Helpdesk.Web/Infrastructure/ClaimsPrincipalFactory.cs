using System.Security.Claims;
using Helpdesk.Web.Contracts;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Helpdesk.Web.Infrastructure;

public static class ClaimsPrincipalFactory
{
    public static ClaimsPrincipal Create(AuthenticatedUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        return new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme));
    }
}
