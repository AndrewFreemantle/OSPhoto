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

    // Just for pictures/photos
    [JsonPropertyName("photo_exif")]
    public PhotoExif PhotoExif { get; set; }

    // both albums and photos
    [JsonPropertyName("thumb_size")]
    public Thumbnails Thumbnails { get; set; }
}
