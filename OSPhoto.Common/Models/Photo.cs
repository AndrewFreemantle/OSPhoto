using System.Runtime.InteropServices;
using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Models;

public class Photo : ItemBase
{
    public new static string IdPrefix => "photo_";

    public Photo(string mediaPath, FileInfo fileInfo) : base(fileInfo)
    {
        Id = GetIdForPath(mediaPath, fileInfo, IdPrefix);
        Type = "photo";

        var imageInfo = SixLabors.ImageSharp.Image.Identify(fileInfo.FullName);
        var mTime = ((DateTimeOffset)fileInfo.LastWriteTimeUtc).ToUnixTimeSeconds();

        Info = new ItemInfo(fileInfo, imageInfo);

        Additional = new ItemAdditional
        {
            PhotoExif = new PhotoExif(imageInfo.Metadata.ExifProfile),
            AlbumPermission = new Permission(),
            Thumbnails = new Thumbnails
            {
                Small = new ThumbnailInfo(200,200, mTime)
                , Large = new ThumbnailInfo(200,200, mTime)
                , Signature = fileInfo.FullName.ToHex()
            }
        };

        ThumbnailStatus = "small,large";
    }
}
