using OSPhoto.Common.Extensions;

namespace OSPhoto.Common;

public class Directory : ItemBase
{
    private const string IdPrefix = "album_";

    public Directory(string contentRootPath, DirectoryInfo dirInfo) : base(dirInfo)
    {
        // TODO: naive implementation - needs _ separated path segments, each Hex encoded...
        Id = GetIdForPath(contentRootPath, dirInfo);
        Type = "album";

        Info = new ItemInfo(Name, Name, Name);

        // TODO: Store (and then retrieve) the image to be used for this Album/Directory's thumbnail in a database
        Additional = new Additional
        {
            Thumbnails = new Thumbnails()
            {
                Small = new ThumbnailInfo(200, 200, 1484649478)
                , Signature = dirInfo.FullName.ToHex()
            }

        };
        ThumbnailStatus = "small";
    }

    public static string GetIdForPath(string contentRootPath, DirectoryInfo dirInfo)
    {
        var rootDirInfo = new DirectoryInfo(contentRootPath.TrimEnd(System.IO.Path.DirectorySeparatorChar));

        var parts = new List<string>();
        var dir = dirInfo;
        while (dir != null && dir.FullName != rootDirInfo.FullName)
        {
            parts.Add(dir.Name.ToHex());
            dir = dir.Parent;
        }

        parts.Reverse();

        return $"{IdPrefix}{string.Join("_", parts)}";
    }

    public static string GetPathFromId(string id)
    {
        if (string.IsNullOrEmpty(id))
            return string.Empty;

        if (!id.StartsWith(IdPrefix))
            throw new ArgumentException($"value must begin with '{IdPrefix}'", nameof(id));

        var parts = id[IdPrefix.Length..]
            .Split('_', StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.FromHex())
            .ToArray();

        return System.IO.Path.Combine(parts);
    }
}
