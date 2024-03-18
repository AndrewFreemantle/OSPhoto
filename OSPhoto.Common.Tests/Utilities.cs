using Microsoft.EntityFrameworkCore;
using OSPhoto.Common.Database;

namespace OSPhoto.Common.Tests;

public static class Utilities
{
    public static ApplicationDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseSqlite("Data Source=:memory:; Version=3")
            .Options;
        return new ApplicationDbContext(options);
    }
}
