namespace OSPhoto.Common.Database.Models;

[Table("photos")]
public class Photo(string id, string path, string title, string? description = null, int? importedShareId = null)
{
    [Key]
    public string Id { get; set; } = id;
    public string Path { get; set; } = path;
    public string Title { get; set; } = title;
    public string? Description { get; set; } = description;
    public int? ImportedShareId { get; set; } = importedShareId;

    public DateTime UpdatedDate { get; set; } = DateTime.UtcNow;
}
