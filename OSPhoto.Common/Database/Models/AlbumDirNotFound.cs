using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Database.Models;

/// <summary>
/// Represents metadata of a photo that was imported but the file to which it pertains wasn't found in the filesystem at the time (<see cref="UpdatedDate"/>)
/// </summary>
[Table("albums_dir_not_found")]
public class AlbumDirNotFound()
{
    [Key]
    public int Id { get; set; }
    public string ShareName { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    public string ImportFilename { get; set; }
    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;

    /// <inheritdoc />
    public AlbumDirNotFound(CsvPhotoShareRecord importRecord, string importFilename): this()
    {
        Id = importRecord.Id;
        ShareName = importRecord.ShareName;
        Title = importRecord.Title;
        Description = importRecord.Description;

        ImportFilename = importFilename;
    }
}
