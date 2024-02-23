namespace OSPhoto.Common.Models;

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
