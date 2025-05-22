using System.Drawing;
using FFMpegCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using File = System.IO.File;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;
using Microsoft.Extensions.Options;
using OSPhoto.Common.Configuration;

namespace OSPhoto.Common.Services;

public class VideoService(ApplicationDbContext dbContext, IFileSystem fileSystem, IOptions<AppSettings> settings, ILogger<VideoService> logger) : ServiceBase(dbContext, fileSystem, settings, logger), IVideoService
{
    public async Task<Stream> GetThumbnail(string id, string? size = "small")
    {
        var videoThumbTempPath = Path.GetTempFileName();
        var videoThumbTempPngPath = $"{videoThumbTempPath}.png";

        var videoThumbSize = new Size(-1, size == "small" ? 200 : 480);

        try
        {
            if (!id.StartsWith(Video.IdPrefix))
                return await Task.FromResult(Stream.Null);

            var videoPath = Path.Combine(_mediaPath, ItemBase.GetPathFromId(id));

            await FFMpeg.SnapshotAsync(videoPath, videoThumbTempPngPath, videoThumbSize, TimeSpan.FromSeconds(0));

            var memoryStream = new MemoryStream(File.ReadAllBytes(videoThumbTempPngPath));

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream;
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Video GetThumbnail failed");
            return Stream.Null;
        }
        finally
        {
            if (File.Exists(videoThumbTempPngPath))
                File.Delete(videoThumbTempPngPath);

            if (File.Exists(videoThumbTempPath))
                File.Delete(videoThumbTempPath);
        }
    }
}
