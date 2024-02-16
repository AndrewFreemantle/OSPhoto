namespace OSPhoto.Common;

public abstract class ItemBase
{
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
    public Additional Additional { get; set; }

    // TODO: this can be readonly computed by inspecting Additional.Thumbnails
    /// <summary>
    /// CSV of thumbnail sizes; e.g.: "preview,small,large"
    /// </summary>
    [JsonPropertyName("thumbnail_status")]
    public string ThumbnailStatus { get; set; }


    // Additional properties for development/debug
    public string Name { get; }
    public string Path { get; }
}
