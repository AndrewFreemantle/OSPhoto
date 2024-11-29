using Microsoft.EntityFrameworkCore;
using OSPhoto.Common.Database;
using OSPhoto.Common.Database.Models;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Services;

public class CommentService(ApplicationDbContext dbContext, ILogger<CommentService> logger) : ICommentService
{
    public async Task<int> Create(string mediaId, string text, string name, string? email)
    {
        try
        {
            var result = await dbContext.Comments.AddAsync(new Comment(mediaId, text, name, email));
            await dbContext.SaveChangesAsync();
            logger.LogInformation($"Comment added: {result.Entity}");
            return result.Entity.Id;
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Error adding Comment: mediaId: {mediaId}, text: {text}, name: {name}, email: {email}");
            return -1;
        }
    }

    public async Task<List<Comment>> Get(string mediaId)
    {
        return await dbContext.Comments
            .Where(c => c.MediaId == mediaId)
            .OrderByDescending(c => c.CreatedUtc)
            .ToListAsync();
    }

    public async Task<MoveResult> Move(string oldMediaId, string newMediaId)
    {
        try
        {
            using (var trans = await dbContext.Database.BeginTransactionAsync())
            {
                await dbContext.Comments
                    .Where(c => c.MediaId == oldMediaId)
                    .ExecuteUpdateAsync(update => update
                        .SetProperty(e => e.MediaId, newMediaId));

                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error moving comments for {oldId} to {newId}", oldMediaId, newMediaId);
            return new MoveResult(false);
        }

        return new MoveResult(true, newMediaId);
    }

    public async Task Delete(string mediaId)
    {
        try
        {
            using (var trans = await dbContext.Database.BeginTransactionAsync())
            {
                await dbContext.Comments
                    .Where(c => c.MediaId == mediaId)
                    .ExecuteDeleteAsync();

                await dbContext.SaveChangesAsync();
                await trans.CommitAsync();
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting comments for {id}", mediaId);
        }
    }
}
