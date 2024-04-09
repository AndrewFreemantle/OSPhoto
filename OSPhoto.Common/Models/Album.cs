using Microsoft.EntityFrameworkCore;
using OSPhoto.Common.Database;
using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Models;

public class Album : ItemBase
{
    public new static string IdPrefix => "album_";

    public Album(string mediaPath, DirectoryInfo dirInfo, ApplicationDbContext dbContext) : base(dirInfo)
    {
        Id = $"{IdPrefix}{dirInfo.FullName[mediaPath.Length..].TrimStart(System.IO.Path.DirectorySeparatorChar).ToHex()}";
        Type = "album";

        // do we have metadata and/or a cover photo for this album/directory?
        var albumRecord = dbContext.Albums.FirstOrDefault(a => a.Id == Id);

        Info = new ItemInfo(Name, Name, albumRecord?.Title, albumRecord?.Description);
        Additional = new ItemAdditional();
        ThumbnailStatus = (albumRecord != null && !string.IsNullOrEmpty(albumRecord.CoverPhotoId)) ? "small,large" : "default";
    }
}
