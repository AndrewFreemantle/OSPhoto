namespace OSPhoto.Common.Models;

public class PhotoExifGps(double? latitude, double? longitude)
{
    [JsonPropertyName("lat")]
    public string? Latitude { get; set; } = latitude.ToString();
    [JsonPropertyName("lng")]
    public string? Longitude { get; set; } = longitude.ToString();
}
