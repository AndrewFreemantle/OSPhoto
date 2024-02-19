using Microsoft.Extensions.Logging;
using OSPhoto.Common.Exceptions;
using OSPhoto.Common.Extensions;
using OSPhoto.Common.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace OSPhoto.Common;

public class AlbumService(string contentRootPath) : IAlbumService
{
    public string ContentRootPath { get; } = contentRootPath;
    private ILogger Logger;

    public int ThumbnailWidthInPixels { get; } =
        int.Parse(Environment.GetEnvironmentVariable("THUMB_WIDTH_PX") ?? "350");

    public void SetLogger(ILogger logger) => Logger = logger;

    public AlbumResult Get(string id = "")
    {
        try
        {
            var path = string.IsNullOrEmpty(id) ? ContentRootPath : Path.Combine(ContentRootPath, id["album_".Length..].FromHex());

            return new AlbumResult(path
                , ContentRootPath
                , GetContentDirectory(path)
                    .EnumerateFileSystemInfos()
                    .Where(fsi => fsi is DirectoryInfo || ((FileInfo)fsi).IsImageFileType())
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

    public Stream GetThumbnail(string id)
    {
        // grab the file, then return a resized version
        var memoryStream = new MemoryStream();

        var imagePath = Path.Combine(ContentRootPath, ItemBase.GetPathFromId(id, "photo_"));

        using (var image = SixLabors.ImageSharp.Image.Load(imagePath))
        {
            image.Mutate(x => x.Resize(ThumbnailWidthInPixels, 0));
            image.Save(memoryStream, new JpegEncoder());
            memoryStream.Position = 0;
            return memoryStream;
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
            return new Image(ContentRootPath, fileInfo);

        // shouldn't occur, but handle it anyway...
        return new File(fsInfo.Name, itemPath);
    }
}
