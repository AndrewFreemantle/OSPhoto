using CsvHelper.Configuration.Attributes;

namespace OSPhoto.Common.Services.Models;

public class CsvPhotoImageRecord
{
    // id,path,name,title,description,album,size,resolutionx,resolutiony,camera_make,camera_model,exposure,aperture,iso,timetaken,updated,version,create_time,gps,disabled,shareid,privilege_shareid,rotation,lat,lng,focal_length_v2,lens_v2,flash_v2,rating
    [Name("id")]
    public int Id { get; set; }
    [Name("path")]
    public string Path { get; set; }
    [Name("title")]
    public string? Title { get; set; }
    [Name("description")]
    public string? Description { get; set; }

    [Name("camera_make")]
    public string? CameraMake { get; set; }
    [Name("camera_model")]
    public string? CameraModel { get; set; }
    [Name("timetaken")]
    public string? TimeTaken { get; set; }
    [Name("shareid")]
    public int? ShareId { get; set; }
}
