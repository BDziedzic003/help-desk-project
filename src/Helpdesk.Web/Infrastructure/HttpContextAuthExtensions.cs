using Helpdesk.Web.Contracts;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Helpdesk.Web.Infrastructure;

public static class HttpContextAuthExtensions
{
    public static Task SignInUserAsync(this HttpContext httpContext, AuthenticatedUser user) =>
        httpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            ClaimsPrincipalFactory.Create(user));

    public static Task SignOutUserAsync(this HttpContext httpContext) =>
        httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
