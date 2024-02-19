using OSPhoto.Common.Extensions;

namespace OSPhoto.Common;

public abstract class ItemBase
{
    public abstract string IdPrefix { get; }

    public ItemBase(FileSystemInfo fsInfo)
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


    public string GetIdForPath(string contentRootPath, FileSystemInfo fsInfo)
    {
        var rootDirInfo = new DirectoryInfo(contentRootPath.TrimEnd(System.IO.Path.DirectorySeparatorChar));
        var parts = new List<string>();

        if (fsInfo is FileInfo fileInfo)
            parts.Add(fileInfo.Name.ToHex());

        var dir = (fsInfo is DirectoryInfo) ? fsInfo as DirectoryInfo : ((FileInfo)fsInfo).Directory;
        while (dir != null && dir.FullName != rootDirInfo.FullName)
        {
            parts.Add(dir.Name.ToHex());
            dir = dir.Parent;
        }

        parts.Reverse();

        return $"{IdPrefix}{string.Join("_", parts)}";
    }

    public static string GetPathFromId(string id, string idPrefix)
    {
        if (string.IsNullOrEmpty(id))
            return string.Empty;

        if (!id.StartsWith(idPrefix))
            throw new ArgumentException($"value must begin with '{idPrefix}'", nameof(id));

        var parts = id[idPrefix.Length..]
            .Split('_', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.FromHex())
            .ToArray();

        return System.IO.Path.Combine(parts);
    }


    // Additional properties for development/debug
    public string Name { get; }
    public string Path { get; }
}
