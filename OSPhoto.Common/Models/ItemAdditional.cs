namespace OSPhoto.Common.Models;

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

    // Just for images/photos
    [JsonPropertyName("photo_exif")]
    public PhotoExif PhotoExif { get; set; }

    // Just for videos
    [JsonPropertyName("video_codec")]
    public VideoCodec VideoCodec { get; set; }
    [JsonPropertyName("video_quality")]
    public VideoQuality[] VideoQuality { get; set; }

    // both albums and photos
    [JsonPropertyName("thumb_size")]
    public Thumbnails Thumbnails { get; set; }
}
