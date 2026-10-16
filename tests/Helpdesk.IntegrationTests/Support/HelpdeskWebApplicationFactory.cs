using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;

namespace Helpdesk.IntegrationTests.Support;

/// <summary>
/// Starts the real application on a throw-away SQLite file, so every test class works on a fresh, seeded database.
/// </summary>
public class HelpdeskWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string AdminUsername = "admin";
    public const string AdminPassword = "admin";

    private readonly string _databasePath = Path.Combine(Path.GetTempPath(), $"helpdesk-tests-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:Default", $"Data Source={_databasePath}");
        builder.UseSetting("AdminSeed:Username", AdminUsername);
        builder.UseSetting("AdminSeed:Password", AdminPassword);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (!disposing)
        {
            return;
        }

        SqliteConnection.ClearAllPools();
        foreach (var path in new[] { _databasePath, _databasePath + "-shm", _databasePath + "-wal" })
        {
            File.Delete(path);
        }
    }
}
