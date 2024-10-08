using FFMpegCore;
using OSPhoto.Common.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;

namespace OSPhoto.Common.Models;

public class ItemInfo
{
    private ItemInfo(FileInfo fileInfo, string? title, string? description)
    {
        // common file info properties
        SharePath = fileInfo.DirectoryName;
        Name = fileInfo.Name;
        Title = title ?? fileInfo.Name;
        Description = description ?? string.Empty;

        CreateDate = fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss");
        Size = fileInfo.Length;
    }

    public ItemInfo(string sharePath, string name, string title, string description = "")
    {
        SharePath = sharePath;
        Name = name;
        Title = title;
        Description = description;
    }

    public ItemInfo(FileInfo fileInfo, ImageInfo imageInfo, string? title = null, string? description = null) : this(fileInfo, title, description)
    {
        // photo properties
        if (imageInfo.Metadata.ExifProfile == null)
            return;

        var exif = imageInfo.Metadata.ExifProfile;

        TakenDate = exif.GetValueString(ExifTag.DateTimeOriginal)
                    ?? exif.GetValueString(ExifTag.DateTime)
                    ?? fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss");

        ResolutionX = imageInfo.Width;
        ResolutionY = imageInfo.Height;

        Rotation = exif.GetValueUShort(ExifTag.Orientation);
        Rotated = Rotation != 1;

        Latitude = exif.GetGpsLatitudeAsDecimalDegrees();
        Longitude = exif.GetGpsLongitudeAsDecimalDegrees();
    }

    public ItemInfo(FileInfo fileInfo, IMediaAnalysis videoInfo, string? title = null, string? description = null) : this(fileInfo, title, description)
    {
        // video properties
        TakenDate = fileInfo.CreationTimeUtc.ToString("yyyy-MM-dd HH:mm:ss");

        Duration = videoInfo.Duration.TotalSeconds;
        Rotation = videoInfo.PrimaryVideoStream.Rotation;
    }

    [JsonPropertyName("sharepath")]
    public string SharePath { get; set; }
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
    public int? Rotation { get; set; }
    [JsonPropertyName("lat")]
    public double? Latitude { get; set; }
    [JsonPropertyName("lng")]
    public double? Longitude { get; set; }
    public int Rating { get; set; } = 0;

    public double Duration { get; set; } = 0;
}
