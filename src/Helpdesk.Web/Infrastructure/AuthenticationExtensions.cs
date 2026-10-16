using Helpdesk.Web.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Helpdesk.Web.Infrastructure;

public static class AuthenticationExtensions
{
    private const string ApiPathPrefix = "/api";

    public static IServiceCollection AddHelpdeskAuthentication(this IServiceCollection services)
    {
        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;
                options.Events.OnRedirectToLogin = context => RespondWithStatusForApi(context, StatusCodes.Status401Unauthorized);
                options.Events.OnRedirectToAccessDenied = context => RespondWithStatusForApi(context, StatusCodes.Status403Forbidden);
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(AuthPolicies.AdminOnly, policy => policy.RequireRole(nameof(Role.Admin)));

        return services;
    }

    /// <summary>
    /// API clients get a plain status code instead of a redirect to the login page.
    /// </summary>
    private static Task RespondWithStatusForApi(RedirectContext<CookieAuthenticationOptions> context, int statusCode)
    {
        if (context.Request.Path.StartsWithSegments(ApiPathPrefix))
        {
            context.Response.StatusCode = statusCode;
        }
        else
        {
            context.Response.Redirect(context.RedirectUri);
        }

        return Task.CompletedTask;
    }
}
