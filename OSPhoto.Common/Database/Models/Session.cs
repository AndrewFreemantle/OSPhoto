namespace OSPhoto.Common.Database.Models;

[Table("sessions")]
public class Session(string username)
{
    [Key]
    public string SessionId { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public string Username { get; set; } = username;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
