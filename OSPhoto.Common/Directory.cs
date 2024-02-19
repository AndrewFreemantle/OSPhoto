using OSPhoto.Common.Extensions;

namespace OSPhoto.Common;

public class Directory : ItemBase
{
    public override string IdPrefix => "album_";

    public Directory(string contentRootPath, DirectoryInfo dirInfo) : base(dirInfo)
    {
        Id = $"{IdPrefix}{dirInfo.FullName[contentRootPath.Length..].TrimStart(System.IO.Path.DirectorySeparatorChar).ToHex()}";
        Type = "album";

        Info = new ItemInfo(Name, Name, Name);
        var mTime = ((DateTimeOffset)dirInfo.LastWriteTimeUtc).ToUnixTimeSeconds();

        // TODO: Store (and then retrieve) the image to be used for this Album/Directory's thumbnail in a database
        Additional = new ItemAdditional
        {
            Thumbnails = new Thumbnails()
            {
                // Small = new ThumbnailInfo(200, 200, mTime)
                // , Signature = dirInfo.FullName.ToHex()
            }

        };
        ThumbnailStatus = "default";
    }
}
