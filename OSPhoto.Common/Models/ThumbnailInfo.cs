namespace OSPhoto.Common.Models;

public class ThumbnailInfo(int x, int y, long modifiedTime)
{
    /// <summary>
    /// Returns the system-wide Thumbnail pixel width
    /// </summary>
    public static int ThumbnailWidthInPixels => int.Parse(Environment.GetEnvironmentVariable("THUMB_WIDTH_PX") ?? "500");

    [JsonPropertyName("resolutionx")]
    public int ResolutionX { get; set; } = x;

    [JsonPropertyName("resolutiony")]
    public int ResolutionY { get; set; } = y;

    [JsonPropertyName("mtime")]
    public long ModifiedTime { get; set; } = modifiedTime;
}
