using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OSPhoto.Common.Database.Models;

[Table("users")]
public class User
{
    [Key]
    public string Username { get; set; }
    public string Password { get; set; }
}
