using System.Text.Json.Serialization;
namespace OSPhoto.Common.Models;
using OSPhoto.Common.Configuration;
using Microsoft.Extensions.Options;

public class ThumbnailInfo
{
    [JsonPropertyName("resolutionx")]
    public int ResolutionX { get; set; }
    [JsonPropertyName("resolutiony")]
    public int ResolutionY { get; set; }
    [JsonPropertyName("mtime")]
    public long ModifiedTime { get; set; }

    public ThumbnailInfo(int x, int y, long modifiedTime)
    {
        ResolutionX = x;
        ResolutionY = y;
        ModifiedTime = modifiedTime;
    }
}
