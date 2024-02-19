namespace OSPhoto.Common;

public class AlbumResponseData
{
    public int Total => Items.Length;
    public int Offset { get; set; }
    public Item[] Items { get; set; }
}

public class Item(string id, string type, ItemInfo info, ItemAdditional itemAdditional, string thumbnailStatus)
{
    /// <summary>
    /// Item identifier, e.g.: "album_6113a5051b1741b3885e61961c5a9485"
    /// </summary>
    public string Id { get; set; } = id;

    // TODO: convert Type into an enum, but will need a custom Json Converter too
    //  https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to?pivots=dotnet-6-0
    /// <summary>
    /// This item's type, e.g.: "album", "photo"
    /// </summary>
    public string Type { get; set; } = type;

    public ItemInfo Info { get; set; } = info;
    public ItemAdditional ItemAdditional { get; set; } = itemAdditional;

    // TODO: this can be readonly computed by inspecting Additional.Thumbnails
    /// <summary>
    /// CSV of thumbnail sizes; e.g.: "preview,small,large"
    /// </summary>
    [JsonPropertyName("thumbnail_status")]
    public string ThumbnailStatus { get; set; } = thumbnailStatus;
}

public class ItemAdditional
{
    public ItemAdditional()
    {
        AlbumPermission = new Permission();
        Thumbnails = new Thumbnails();
    }

    // Just for albums...
    [JsonPropertyName("album_permission")]
    public Permission AlbumPermission { get; set; }

    // Just for pictures/photos
    [JsonPropertyName("photo_exif")]
    public PhotoExif PhotoExif { get; set; }

    // both albums and photos
    [JsonPropertyName("thumb_size")]
    public Thumbnails Thumbnails { get; set; }
}
