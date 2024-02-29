using System.Net;
using OSPhoto.Common.Extensions;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace OSPhoto.Common.Models;

public class PhotoExif
{
    public PhotoExif(ExifProfile? exif)
    {
        if (exif == null) return;

        TakenDate = exif.GetValueString(ExifTag.DateTimeOriginal)
                    ?? exif.GetValueString(ExifTag.DateTime);
        Camera = exif.GetValueString(ExifTag.Make);
        CameraModel = exif.GetValueString(ExifTag.Model);
        Exposure = $"{exif.GetValueString(ExifTag.ExposureTime)} sec";
        Aperture = $"f/{exif.GetValueDouble(ExifTag.FNumber):F1}";
        ISO = exif.GetValueUShortArray(ExifTag.ISOSpeedRatings);
        GPS = new PhotoExifGps(
            exif.GetGpsLatitudeAsDecimalDegrees(),
            exif.GetGpsLongitudeAsDecimalDegrees());
        FocalLength = exif.GetValueString(ExifTag.FocalLength);
        Lens = $"{exif.GetValueString(ExifTag.LensMake)} {exif.GetValueString(ExifTag.LensModel)}";
        Flash = exif.GetValueString(ExifTag.Flash);
    }

    [JsonPropertyName("takendate")]
    public string? TakenDate { get; set; }
    public string? Camera { get; set; }
    [JsonPropertyName("camera_model")]
    public string? CameraModel { get; set; }
    public string? Exposure { get; set; }
    public string? Aperture { get; set; }
    [JsonPropertyName("iso")]
    public uint? ISO { get; set; }
    [JsonPropertyName("gps")]
    public PhotoExifGps? GPS { get; set; }
    [JsonPropertyName("focal_length")]
    public string? FocalLength { get; set; }
    public string? Lens { get; set; }
    public string? Flash { get; set; }
}
