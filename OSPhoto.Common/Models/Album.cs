using Microsoft.EntityFrameworkCore;
using OSPhoto.Common.Database;
using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Models;

public class Album : ItemBase
{
    public new static string IdPrefix => "album_";

    public Album(string mediaPath, IDirectoryInfo dirInfo, ApplicationDbContext dbContext) : base(dirInfo)
    {
        var sharePath = dirInfo.FullName[mediaPath.Length..].TrimStart(System.IO.Path.DirectorySeparatorChar);
        Id = $"{IdPrefix}{sharePath.ToHex()}";
        Type = "album";

        // do we have metadata and/or a cover photo for this album/directory?
        var albumRecord = dbContext.Albums.FirstOrDefault(a => a.Id == Id);

        Info = new ItemInfo(sharePath, Name, albumRecord?.Title, albumRecord?.Description);
        Additional = new ItemAdditional();

        // do we have a specific album thumbnail?
        ThumbnailStatus = (
            (albumRecord != null && !string.IsNullOrEmpty(albumRecord.CoverPhotoId))    // user specified album cover exists
            || dirInfo.EnumerateFiles().Any(f => f.IsImageFileType())            // album dir contains at least one image
            ) ? "small,large" : "default";
    }
}
