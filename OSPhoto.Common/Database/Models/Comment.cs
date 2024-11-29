using Microsoft.EntityFrameworkCore;

namespace OSPhoto.Common.Database.Models;

[Table("comments")]
[Index(nameof(MediaId), IsUnique = false)]
public class Comment(string mediaId, string text, string name, string? email = null)
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public string MediaId { get; set; } = mediaId;

    public string Text { get; set; } = text;
    public string Name { get; set; } = name;
    public string? Email { get; set; } = email;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
