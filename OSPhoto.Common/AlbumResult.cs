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
        Path = path;
        ContentRootPath = contentRootPath;
        Items = contents;
    }
}
