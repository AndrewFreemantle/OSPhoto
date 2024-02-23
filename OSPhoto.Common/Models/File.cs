using OSPhoto.Common.Models;

namespace OSPhoto.Common;

public class File : ItemBase
{
    public override string IdPrefix => "file_";

    public File(string name, string path) : base(name, path)
    {
    }
}
