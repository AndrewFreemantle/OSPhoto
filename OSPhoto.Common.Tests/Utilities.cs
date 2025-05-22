using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using OSPhoto.Common.Database;

namespace OSPhoto.Common.Tests;

public static class Utilities
{
    private static SqliteConnection? _connection;

    public static ApplicationDbContext GetInMemoryDbContext()
    {
        // Ensure an open SQLite connection for in-memory database
        if (_connection == null)
        {
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();
        }

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite(_connection) // Use the open connection
            .Options;

        var dbContext = new ApplicationDbContext(options);

        // Ensure database schema is created
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}
