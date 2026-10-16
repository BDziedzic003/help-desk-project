using Helpdesk.Web.Domain;

namespace Helpdesk.Web.Infrastructure;

public static class UserDisplayExtensions
{
    public static string ToDisplayName(this Role role) => role switch
    {
        Role.Admin => "Administrator",
        Role.User => "Użytkownik",
        _ => role.ToString()
    };

    public static string ToBadgeClass(this Role role) => role == Role.Admin ? "bg-danger" : "bg-secondary";
}
