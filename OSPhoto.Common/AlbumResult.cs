namespace OSPhoto.Common;

public class AlbumResult
{
    public int Total => Items.Count();
    public int Offset { get; } = 0;
    public IEnumerable<ItemBase> Items { get; set; }

    // Additional properties for development/debugging
    public string Path { get; }
    public string ContentRootPath { get; }

    public AlbumResult(string path, string contentRootPath, IEnumerable<ItemBase> contents)
    {
        // Location = SplitPathIntoBreadcrumbs(path, contentRootPath);
        Path = path;
        ContentRootPath = contentRootPath;
        Items = contents;
    }

    private IEnumerable<Location> SplitPathIntoBreadcrumbs(string path, string contentRootPath)
    {
        if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(path)) return Enumerable.Empty<Location>();

        var fullPath = new DirectoryInfo(System.IO.Path.Combine(contentRootPath, path));
        var location = new List<Location>();

        var currentPath = fullPath;
        while (currentPath != null)
        {
            location.Add(new Location(currentPath, contentRootPath));
            currentPath = currentPath.Parent;

            // Stop walking when we reach the content root
            if (currentPath != null && currentPath.FullName == contentRootPath) currentPath = null;
        }

        location.Reverse();
        return location;
    }
}
