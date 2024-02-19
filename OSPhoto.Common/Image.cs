using OSPhoto.Common.Extensions;

namespace OSPhoto.Common;

public class Image : ItemBase
{
    public override string IdPrefix => "photo_";

    public Image(string contentRootPath, FileInfo fileInfo) : base(fileInfo)
    {
        Id = GetIdForPath(contentRootPath, fileInfo);
        Type = "photo";

        var imageInfo = SixLabors.ImageSharp.Image.Identify(Path);
        var mTime = ((DateTimeOffset)fileInfo.LastWriteTimeUtc).ToUnixTimeSeconds();

        Info = new ItemInfo(Name, Name, Name)
        {
            CreateDate = fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss")
            , TakenDate = fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss") // TODO: get this from EXIF
            , Size = fileInfo.Length
            , ResolutionX = imageInfo.Width
            , ResolutionY = imageInfo.Height
        };

        Additional = new ItemAdditional
        {
            Thumbnails = new Thumbnails()
            {
                Small = new ThumbnailInfo(200, 200, mTime)
                , Large = new ThumbnailInfo(200, 200, mTime)
                , Signature = fileInfo.FullName.ToHex()
            }
        };

        ThumbnailStatus = "small,large";
    }

    public Image(string name, string path, string contentType) : base(name, path)
    {
        ContentType = contentType;
    }

    public string ContentType { get; }
}
