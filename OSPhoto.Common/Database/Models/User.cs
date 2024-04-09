namespace OSPhoto.Common.Database.Models;

[Table("users")]
public class User
{
    [Key]
    public string Username { get; set; }
    public string Password { get; set; }
}
