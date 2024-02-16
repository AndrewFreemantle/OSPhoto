namespace OSPhoto.Common;

public class Location
{
    public string Name { get; set; }
    public string Path { get; set; }

    public Location(DirectoryInfo directoryInfo, string contentRootPath)
    {
        Name = directoryInfo.Name;
        Path = directoryInfo.FullName.Substring(contentRootPath.Length + 1);
    }
}
