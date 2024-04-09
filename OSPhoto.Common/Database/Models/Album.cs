namespace OSPhoto.Common.Database.Models;

[Table("albums")]
public class Album(string id, string path, string? title = null, string? description = null, string? coverPhotoId = null)
{
    [Key]
    public string Id { get; set; } = id;
    public string Path { get; set; } = path;
    public string? Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public string? CoverPhotoId { get; set; } = coverPhotoId;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
