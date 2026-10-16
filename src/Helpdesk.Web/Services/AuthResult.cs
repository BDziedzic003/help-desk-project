using Helpdesk.Web.Contracts;

namespace Helpdesk.Web.Services;

public record AuthResult(bool Succeeded, AuthenticatedUser? User, string? Error)
{
    public static AuthResult Success(AuthenticatedUser user) => new(true, user, null);

    public static AuthResult Failure(string error) => new(false, null, error);
}
