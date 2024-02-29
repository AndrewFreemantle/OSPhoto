
namespace OSPhoto.Common.Models;

public class Permission
{
    public bool Browse { get; set; } = true;
    public bool Upload { get; set; } = true;
    public bool Manage { get; set; } = true;
}
