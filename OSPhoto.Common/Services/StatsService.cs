using Microsoft.EntityFrameworkCore;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Services;

public class StatsService(ApplicationDbContext dbContext) : IStatsService
{
    public async Task<StatsResponse> Get()
    {
        return new StatsResponse(
            await dbContext.Albums.CountAsync(),
            await dbContext.AlbumsDirNotFound.CountAsync(),

            await dbContext.Photos.CountAsync(),
            await dbContext.PhotosFileNotFound.CountAsync());
    }
}
