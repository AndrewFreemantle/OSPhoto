namespace OSPhoto.Common;

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