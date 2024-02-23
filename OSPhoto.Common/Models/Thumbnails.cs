namespace OSPhoto.Common.Models;

public class Thumbnails
{
    public ThumbnailInfo Preview { get; set; } = new ThumbnailInfo(0, 0, 0);
    public ThumbnailInfo Small { get; set; } = new ThumbnailInfo(0, 0, 0);
    public ThumbnailInfo Large { get; set; } = new ThumbnailInfo(0, 0, 0);
    [JsonPropertyName("sig")]
    public string Signature { get; set; } = string.Empty;
}
