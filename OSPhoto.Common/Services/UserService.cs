using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Database.Models;
using OSPhoto.Common.Interfaces;

namespace OSPhoto.Common.Services;

public class UserService(ApplicationDbContext dbContext, ILogger<UserService> logger) : IUserService
{
    public async Task<string> LoginAsync(string username, string plainTextPassword)
    {
        try
        {
            var user = await dbContext.Users.FindAsync(username);

            if (user != null && BC.EnhancedVerify(plainTextPassword, user.Password))
            {
                // success - user found and password matches
                return await CreateSessionAsync(username);
            }

            logger.LogInformation("Username or password are incorrect");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception logging in user: {username}", username);
        }

        return string.Empty;
    }

    public async Task<bool> IsSessionIdValidAsync(string sessionId)
    {
        return await dbContext
            .Sessions
            .AnyAsync(s => s.SessionId == sessionId);
    }

    public async Task LogoutAsync(string? sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            return;

        await dbContext
            .Database
            .ExecuteSqlRawAsync("DELETE FROM sessions WHERE SessionId = @p0", sessionId);
    }

    private async Task<string> CreateSessionAsync(string username)
    {
        try
        {
            using (var trans = await dbContext.Database.BeginTransactionAsync())
            {
                // invalidate any existing sessions for this user
                await dbContext
                    .Database
                    .ExecuteSqlRawAsync("DELETE FROM sessions WHERE Username = @p0", username);

                var session = new Session(username);
                dbContext.Sessions.Add(session);

                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();

                return session.SessionId;
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception creating session for {username}", username);
            return string.Empty;
        }
    }
}
