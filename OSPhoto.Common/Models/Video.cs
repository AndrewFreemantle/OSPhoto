using FFMpegCore;
using OSPhoto.Common.Database;
using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Models;

public class Video : ItemBase
{
    public new static string IdPrefix => "video_";

    public Video(string mediaPath, IFileInfo fileInfo, ApplicationDbContext dbContext) : base(fileInfo)
    {
        Id = GetIdForPath(mediaPath, fileInfo, IdPrefix);
        Type = "video";

        var videoInfo = FFProbe.Analyse(fileInfo.FullName);
        var videoRecord = dbContext.Photos.FirstOrDefault(p => p.Id == Id);
        var mTime = ((DateTimeOffset)fileInfo.LastWriteTimeUtc).ToUnixTimeSeconds();
        var videoCodec = new VideoCodec(videoInfo);
        var videoQuality = new[] { new VideoQuality(videoInfo, fileInfo) };

        Info = new ItemInfo(fileInfo, videoInfo, videoRecord?.Title, videoRecord?.Description);

        Additional = new ItemAdditional
        {
            VideoCodec = videoCodec,
            VideoQuality = videoQuality,
            Thumbnails = new Thumbnails
            {
                Small = new ThumbnailInfo(200,200, mTime),
                Large = new ThumbnailInfo(videoCodec.ResolutionX, videoCodec.ResolutionY, mTime),
                Signature = fileInfo.FullName.ToHex()
            }
        };

        ThumbnailStatus = "small,large";
    }
}
