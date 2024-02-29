namespace OSPhoto.Common.Models;

public class File : ItemBase
{
    public new static string IdPrefix => "file_";

    public File(string name, string path) : base(name, path)
    {
    }
}
