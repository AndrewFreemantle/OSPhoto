using OSPhoto.Common.Database;
using OSPhoto.Common.Extensions;

namespace OSPhoto.Common.Models;

public abstract class ItemBase
{
    public static string IdPrefix => "itembase_";

    public ItemBase(IFileSystemInfo fsInfo)
    {
        Name = fsInfo.Name;
        Path = fsInfo.FullName;
    }

    public ItemBase(string name, string path)
    {
        Name = name;
        Path = path;
    }

    /// <summary>
    /// Item identifier, e.g.: "album_6113a5051b1741b3885e61961c5a9485"
    /// </summary>
    public string Id { get; set; }

    // TODO: convert Type into an enum, but will need a custom Json Converter too
    //  https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-6-0
    /// <summary>
    /// This item's type, e.g.: "album", "photo"
    /// </summary>
    public string Type { get; set; }

    public ItemInfo Info { get; set; }
    public ItemAdditional Additional { get; set; }

    // TODO: this can be readonly computed by inspecting Additional.Thumbnails
    /// <summary>
    /// CSV of thumbnail sizes; e.g.: "preview,small,large,default"
    /// </summary>
    [JsonPropertyName("thumbnail_status")]
    public string ThumbnailStatus { get; set; }


    public static string GetIdForPath(string mediaPath, IFileSystemInfo fsInfo, string idPrefix)
    {
        var rootDirInfo = new FileSystem().DirectoryInfo.New(mediaPath.TrimEnd(System.IO.Path.DirectorySeparatorChar));
        var parts = new List<string>();

        if (fsInfo is IFileInfo fileInfo)
            parts.Add(fileInfo.Name.ToHex());

        var dir = fsInfo as IDirectoryInfo ?? ((IFileInfo)fsInfo).Directory;
        while (dir != null && dir.FullName != rootDirInfo.FullName)
        {
            parts.Add(dir.Name.ToHex());
            dir = dir.Parent;
        }

        parts.Reverse();

        return $"{idPrefix}{string.Join("_", parts)}";
    }

    public static string GetPathFromId(string id)
    {
        if (string.IsNullOrEmpty(id))
            return string.Empty;

        var validPrefixes = new List<string> { Album.IdPrefix, Photo.IdPrefix, Video.IdPrefix };

        var idPrefix = validPrefixes.FirstOrDefault(id.StartsWith);

        if (string.IsNullOrEmpty(idPrefix))
            throw new ArgumentException($"Media Id's must begin with '{string.Join(",", validPrefixes)}'", nameof(id));

        var parts = id[idPrefix.Length..]
            .Split('_', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.FromHex())
            .ToArray();

        return System.IO.Path.Combine(parts);
    }

    public static ItemBase ConvertToItemBase(IFileSystemInfo fsInfo, string mediaPath, ApplicationDbContext dbContext)
    {
        var itemPath = fsInfo.FullName.Substring(mediaPath.Length);

        if (fsInfo is IDirectoryInfo directoryInfo)
            return new Album(mediaPath, directoryInfo, dbContext);

        if (fsInfo is IFileInfo imageFileInfo && imageFileInfo.IsImageFileType())
            return new Photo(mediaPath, imageFileInfo, dbContext);

        if (fsInfo is IFileInfo videoFileInfo && videoFileInfo.IsVideoFileType())
            return new Video(mediaPath, videoFileInfo, dbContext);

        // shouldn't occur, but handle it anyway...
        return new File(fsInfo.Name, itemPath);
    }


    // Additional properties for development/debug
    public string Name { get; }
    public string Path { get; }
}
