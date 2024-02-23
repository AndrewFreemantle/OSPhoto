namespace OSPhoto.Common.Models;

public class AlbumResult(IEnumerable<ItemBase> items, string path, string mediaPath)
{
    public int Total => Items.Count();
    // TODO: implement paging & offset querying
    public int Offset { get; } = 0;
    public IEnumerable<ItemBase> Items => items;

    // Additional properties for development/debugging
    public string Path => path;
    public string MediaPath => mediaPath;
}
