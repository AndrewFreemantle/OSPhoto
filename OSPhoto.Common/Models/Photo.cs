using OSPhoto.Common.Database;
using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Models;

public class Photo : ItemBase
{
    public new static string IdPrefix => "photo_";

    public Photo(string mediaPath, IFileInfo fileInfo, ApplicationDbContext dbContext) : base(fileInfo)
    {
        Id = GetIdForPath(mediaPath, fileInfo, IdPrefix);
        Type = "photo";

        var imageInfo = SixLabors.ImageSharp.Image.Identify(fileInfo.FullName);
        var photoRecord = dbContext.Photos.FirstOrDefault(p => p.Id == Id);
        var mTime = ((DateTimeOffset)fileInfo.LastWriteTimeUtc).ToUnixTimeSeconds();

        Info = new ItemInfo(fileInfo, imageInfo, photoRecord?.Title, photoRecord?.Description);

        Additional = new ItemAdditional
        {
            PhotoExif = new PhotoExif(imageInfo.Metadata.ExifProfile),
            AlbumPermission = new Permission(),
            Thumbnails = new Thumbnails
            {
                Small = new ThumbnailInfo(200,200, mTime),
                Large = new ThumbnailInfo(200,200, mTime),
                Signature = fileInfo.FullName.ToHex()
            }
        };

        ThumbnailStatus = "small,large";
    }
}
