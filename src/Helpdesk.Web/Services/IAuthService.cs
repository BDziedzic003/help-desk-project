namespace Helpdesk.Web.Services;

public interface IAuthService
{
    Task<AuthResult> AuthenticateAsync(string username, string password);
}
