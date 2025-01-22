using Microsoft.EntityFrameworkCore;
using OSPhoto.Common.Database.Models;

namespace OSPhoto.Common.Database;

public class ApplicationDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Session> Sessions => Set<Session>();
    public DbSet<Photo> Photos => Set<Photo>();
    public DbSet<Album> Albums => Set<Album>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PhotoFileNotFound> PhotosFileNotFound => Set<PhotoFileNotFound>();
    public DbSet<AlbumDirNotFound> AlbumsDirNotFound => Set<AlbumDirNotFound>();
    public DbSet<CommentFileNotFound> CommentsFileNotFound => Set<CommentFileNotFound>();

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    #region Migration CLI Support
    // dotnet ef migrations --project "OSPhoto.Common" {command}
    public ApplicationDbContext()
    {
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .UseSqlite($"Data Source={Environment.GetEnvironmentVariable("DATABASE_PATH")}");
    }
    #endregion

    public void CreateDatabase(string? databasePath = null)
    {
        // ensure the database path exists if given...
        if (!string.IsNullOrEmpty(databasePath))
            new FileSystem().DirectoryInfo.New(databasePath).Parent!.Create();

        // TODO: try-catch?
        Database.Migrate();
    }

    public void SeedUsers(string? users)
    {
        using (var trans = Database.BeginTransaction())
        {
            try
            {
                // remove all user accounts
                //  (no users == no authentication, any username and password combo will be allowed in)

                Database.ExecuteSqlRaw("DELETE FROM users");

                if (string.IsNullOrEmpty(users))
                    return;

                var envUsers = users.Split(";");
                Users.AddRange(envUsers.Select(s =>
                {
                    var parts = s.Split("=");
                    return new User
                    {
                        Username = parts.First(),
                        Password = BC.EnhancedHashPassword(parts.Last())
                    };
                }));
            }
            catch (Exception)
            {
                // if an attempt was made to add users but it failed, add a random user so all login attempts fail
                //  (an empty users table == no authentication)
                Users.Add(new User
                {
                    Username = "None Shall Pass",
                    Password = BC.EnhancedHashPassword(Guid.NewGuid().ToString())
                });
            }
            finally
            {
                SaveChanges();
                trans.Commit();
            }
        }
    }
}
