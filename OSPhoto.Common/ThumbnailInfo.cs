namespace OSPhoto.Common;

public class ThumbnailInfo(int x, int y, long modifiedTime)
{
    [JsonPropertyName("resolutionx")]
    public int ResolutionX { get; set; } = x;

    [JsonPropertyName("resolutiony")]
    public int ResolutionY { get; set; } = y;

    [JsonPropertyName("mtime")]
    public long ModifiedTime { get; set; } = modifiedTime;
}