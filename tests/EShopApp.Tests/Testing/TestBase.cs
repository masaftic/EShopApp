using EShopApp.Infrastructure.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EShopApp.Tests.Testing;

public abstract class TestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected readonly ApplicationDbContext DbContext;

    protected TestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection)
            .EnableSensitiveDataLogging()
            .Options;

        DbContext = new ApplicationDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    public virtual void Dispose()
    {
        DbContext.Database.EnsureDeleted();
        DbContext.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
