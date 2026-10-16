using Helpdesk.Web.Domain;
using Helpdesk.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Helpdesk.Web.Data;

public class DbSeeder(
    HelpdeskDbContext dbContext,
    IPasswordHasher passwordHasher,
    IOptions<AdminSeedOptions> options)
{
    public async Task SeedAsync()
    {
        var settings = options.Value;
        if (await dbContext.Users.AnyAsync(user => user.Username == settings.Username))
        {
            return;
        }

        dbContext.Users.Add(new User
        {
            Username = settings.Username,
            PasswordHash = passwordHasher.Hash(settings.Password),
            Role = Role.Admin
        });
        await dbContext.SaveChangesAsync();
    }
}
