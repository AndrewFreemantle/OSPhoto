using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSPhoto.Common.Models;

[Table("sessions")]
public class Session(string username)
{
    [Key]
    public string SessionId { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public string Username { get; set; } = username;
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
