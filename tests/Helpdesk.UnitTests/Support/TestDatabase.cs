using Helpdesk.Web.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace Helpdesk.UnitTests.Support;

/// <summary>
/// In-memory SQLite database with the real schema, disposed together with the test.
/// </summary>
public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection;

    public TestDatabase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<HelpdeskDbContext>().UseSqlite(_connection).Options;
        Context = new HelpdeskDbContext(options);
        Context.Database.EnsureCreated();
    }

    public HelpdeskDbContext Context { get; }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}
