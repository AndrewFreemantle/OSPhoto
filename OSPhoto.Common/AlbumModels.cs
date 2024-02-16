namespace OSPhoto.Common;

public class AlbumResponseData
{
    public int Total => Items.Length;
    public int Offset { get; set; }
    public Item[] Items { get; set; }
}

public class Item(string id, string type, ItemInfo info, Additional additional, string thumbnailStatus)
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
    public Additional Additional { get; set; } = additional;

    // TODO: this can be readonly computed by inspecting Additional.Thumbnails
    /// <summary>
    /// CSV of thumbnail sizes; e.g.: "preview,small,large"
    /// </summary>
    [JsonPropertyName("thumbnail_status")]
    public string ThumbnailStatus { get; set; } = thumbnailStatus;
}

public class ItemInfo
{
    public ItemInfo(string sharePath, string name, string title, string description = "")
    {
        SharePath = sharePath;
        Name = name;
        Title = title;
        Description = description;
    }

    [JsonPropertyName("sharepath")]
    public string SharePath { get; set; }
    /// <summary>
    /// Filename?
    /// </summary>
    public string Name { get; set; }
    public string Title { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Hits { get; set; } = "1";
    public string Type { get; set; } = "private";
    public bool Conversion { get; set; } = true;
    [JsonPropertyName("allow_comment")]
    public bool AllowComment { get; set; } = false;
    [JsonPropertyName("allow_embed")]
    public bool AllowEmbed { get; set; } = false;

    // if parent Item.Type == "photo"
    // then the following are populated
    [JsonPropertyName("createdate")]
    public string CreateDate { get; set; }
    [JsonPropertyName("takendate")]
    public string TakenDate { get; set; }
    public long Size { get; set; }
    [JsonPropertyName("resolutionx")]
    public int ResolutionX { get; set; }
    [JsonPropertyName("resolutiony")]
    public int ResolutionY { get; set; }

    public bool Rotated { get; set; } = false;
    [JsonPropertyName("rotate_version")]
    public int RotateVersion { get; set; } = 0;
    public int Rotation { get; set; }
    [JsonPropertyName("lat")]
    public int Latitude { get; set; } = 0;
    [JsonPropertyName("lng")]
    public int Longitude { get; set; } = 0;
    public int Rating { get; set; } = 0;
}

public class Additional
{
    public Additional()
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

public class Thumbnails
{
    public ThumbnailInfo Preview { get; set; }
    public ThumbnailInfo Small { get; set; }
    public ThumbnailInfo Large { get; set; }
    [JsonPropertyName("sig")]
    public string Signature { get; set; }
}

public class ThumbnailInfo(int x, int y, long modifiedTime)
{
    [JsonPropertyName("resolutionx")]
    public int ResolutionX { get; set; } = x;

    [JsonPropertyName("resolutiony")]
    public int ResolutionY { get; set; } = y;

    [JsonPropertyName("mtime")]
    public long ModifiedTime { get; set; } = modifiedTime;
}

public class PhotoExif
{
    [JsonPropertyName("takendate")]
    public string TakenDate { get; set; }
    public string Camera { get; set; }
    [JsonPropertyName("camera_model")]
    public string CameraModel { get; set; }
    public string Exposure { get; set; }
    public string Aperture { get; set; }
    [JsonPropertyName("iso")]
    public int ISO { get; set; }
    [JsonPropertyName("gps")]
    public object GPS { get; set; }
    [JsonPropertyName("focal_length")]
    public string FocalLength { get; set; }
    public string Lens { get; set; }
    public string Flash { get; set; }
}
