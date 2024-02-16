using OSPhoto.Common.Extensions;

namespace OSPhoto.Common;

public class Image : ItemBase
{
    private const string IdPrefix = "photo_";

    public Image(FileInfo fileInfo) : base(fileInfo)
    {
        // TODO: naive implementation - needs _ separated path segments, each Hex encoded...
        Id = $"{IdPrefix}{Name.ToHex()}";
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

        Additional = new Additional
        {
            Thumbnails = new Thumbnails()
            {
                Small = new ThumbnailInfo(200, 200, mTime)
                , Signature = fileInfo.FullName.ToHex()
            }
        };

        ThumbnailStatus = "small";
    }

    public Image(string name, string path, string contentType) : base(name, path)
    {
        ContentType = contentType;
    }

    public string ContentType { get; }
}
