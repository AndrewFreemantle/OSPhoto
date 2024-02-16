using Microsoft.Extensions.Logging;
using OSPhoto.Common.Exceptions;
using OSPhoto.Common.Extensions;
using OSPhoto.Common.Interfaces;

namespace OSPhoto.Common;

public class AlbumService(string contentRootPath) : IAlbumService
{
    public string ContentRootPath { get; } = contentRootPath;
    private ILogger Logger;

    public void SetLogger(ILogger logger) => Logger = logger;

    public AlbumResult Get(string id = "")
    {
        try
        {
            var path = string.IsNullOrEmpty(id) ? ContentRootPath : Directory.GetPathFromId(id);

            return new AlbumResult(path
                , ContentRootPath
                , GetContentDirectory(path)
                    .EnumerateFileSystemInfos()
                    .Select(ConvertToItemBase)
                    .OrderByDescending(item => item.GetType() == typeof(Directory))
                    .ThenBy(item => item.Name));
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception getting album (directory) contents for {id}", id);
            throw new AlbumServiceException(id, ex.Message, ex);
        }
    }

    public Image GetImage(string path)
    {
        try
        {
            var fsInfo = new FileInfo(Path.Combine(ContentRootPath, path));
            var image = ConvertToItemBase(fsInfo);

            return new Image(image.Name, image.Path, fsInfo.ContentType());
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Exception getting image for path {path}", path);
            throw;
        }
    }

    private DirectoryInfo GetContentDirectory(string path = null)
    {
        return new DirectoryInfo(
            string.IsNullOrEmpty(path)
            || string.IsNullOrWhiteSpace(path)
                ? ContentRootPath
                : Path.Combine(ContentRootPath, path));
    }

    private ItemBase ConvertToItemBase(FileSystemInfo fsInfo)
    {
        var itemPath = fsInfo.FullName.Substring(ContentRootPath.Length);

        if (fsInfo is DirectoryInfo directoryInfo)
            return new Directory(ContentRootPath, directoryInfo);

        if (fsInfo is FileInfo fileInfo && fileInfo.IsImageFileType())
            return new Image(fileInfo);

        return new File(fsInfo.Name, itemPath);
    }
}
