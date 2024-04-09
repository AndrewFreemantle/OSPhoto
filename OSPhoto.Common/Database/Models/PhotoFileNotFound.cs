using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Database.Models;

/// <summary>
/// Represents metadata of a photo that was imported but the file to which it pertains wasn't found in the filesystem at the time (<see cref="UpdatedDate"/>)
/// </summary>
[Table("photos_file_not_found")]
public class PhotoFileNotFound()
{
    [Key]
    public int Id { get; set; }
    public string Path { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? CameraMake { get; set; }
    public string? CameraModel { get; set; }
    public string? TimeTaken { get; set; }
    public int? ShareId { get; set; }

    public string ImportFilename { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    /// <inheritdoc />
    public PhotoFileNotFound(CsvPhotoImageRecord importRecord, string importFilename) : this()
    {
        Id = importRecord.Id;
        Path = importRecord.Path;
        Title = importRecord.Title;
        Description = importRecord.Description;
        CameraMake = importRecord.CameraMake;
        CameraModel = importRecord.CameraModel;
        TimeTaken = importRecord.TimeTaken;
        ShareId = importRecord.ShareId;

        ImportFilename = importFilename;
    }
}
