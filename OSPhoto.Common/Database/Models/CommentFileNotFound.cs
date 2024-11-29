using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Database.Models;

/// <summary>
/// Represents a comment that was imported but the media file to which it pertains wasn't found in the filesystem at the time
/// </summary>
[Table("comments_file_not_found")]
public class CommentFileNotFound()
{
    [Key]
    public int Id { get; set; }
    public string Path { get; set; } = string.Empty;
    public string Comment { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

    public string ImportFilename { get; set; } = string.Empty;
    public DateTime ImportDate { get; set; } = DateTime.UtcNow;

    public CommentFileNotFound(CsvPhotoCommentRecord importRecord, DateTime commentDate, string importFilename) : this()
    {
        Id = importRecord.Id;
        Path = importRecord.Path;
        Comment = importRecord.Comment;
        Name = importRecord.Name;
        Email = importRecord.Email;
        CreatedUtc = commentDate;

        ImportFilename = importFilename;
    }
}
