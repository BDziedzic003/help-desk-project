using Helpdesk.Web.Contracts;
using Helpdesk.Web.Data;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.Web.Services;

public class AuthService(HelpdeskDbContext dbContext, IPasswordHasher passwordHasher) : IAuthService
{
    public const string InvalidCredentialsError = "Nieprawidłowa nazwa użytkownika lub hasło.";

    public async Task<AuthResult> AuthenticateAsync(string username, string password)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);

        if (user is null || !passwordHasher.Verify(password, user.PasswordHash))
        {
            return AuthResult.Failure(InvalidCredentialsError);
        }

        return AuthResult.Success(new AuthenticatedUser(user.Id, user.Username, user.Role));
    }
}
