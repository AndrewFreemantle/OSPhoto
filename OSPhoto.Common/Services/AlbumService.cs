using Microsoft.Extensions.Logging;
using OSPhoto.Common.Exceptions;
using OSPhoto.Common.Extensions;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace OSPhoto.Common.Services;

public class AlbumService(ILogger<AlbumService> logger) : IAlbumService
{
    private string MediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");
    private int ThumbnailWidthInPixels => int.Parse(Environment.GetEnvironmentVariable("THUMB_WIDTH_PX") ?? "2000");

    public AlbumResult Get(string id = "")
    {
        try
        {
            var path = string.IsNullOrEmpty(id)
                ? MediaPath
                : Path.Combine(MediaPath, id["album_".Length..].FromHex());

            return new AlbumResult(GetContentDirectory(path)
                .EnumerateFileSystemInfos()
                .Where(fsi => fsi is DirectoryInfo || ((FileInfo)fsi).IsImageFileType())
                .Select(ConvertToItemBase)
                .OrderByDescending(item => item.GetType() == typeof(Album))
                .ThenBy(item => item.Name), path, MediaPath);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception getting album (directory) contents for {id}", id);
            throw new AlbumServiceException(id, ex.Message, ex);
        }
    }

    public Stream GetThumbnail(string id)
    {
        // grab the file, then return a resized version
        var memoryStream = new MemoryStream();

        var imagePath = Path.Combine(MediaPath, ItemBase.GetPathFromId(id, "photo_"));

        using (var image = SixLabors.ImageSharp.Image.Load(imagePath))
        {
            image.Mutate(x => x.Resize(ThumbnailWidthInPixels, 0));
            image.Save(memoryStream, new JpegEncoder());
            memoryStream.Position = 0;
            return memoryStream;
        }
    }

    public Photo GetImage(string path)
    {
        try
        {
            var fsInfo = new FileInfo(Path.Combine(MediaPath, path));
            var image = ConvertToItemBase(fsInfo);

            return new Photo(image.Name, image.Path, fsInfo.ContentType());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception getting image for path {path}", path);
            throw;
        }
    }

    private DirectoryInfo GetContentDirectory(string path = null)
    {
        return new DirectoryInfo(
            string.IsNullOrEmpty(path)
            || string.IsNullOrWhiteSpace(path)
                ? MediaPath
                : Path.Combine(MediaPath, path));
    }

    private ItemBase ConvertToItemBase(FileSystemInfo fsInfo)
    {
        var itemPath = fsInfo.FullName.Substring(MediaPath.Length);

        if (fsInfo is DirectoryInfo directoryInfo)
            return new Album(MediaPath, directoryInfo);

        if (fsInfo is FileInfo fileInfo && fileInfo.IsImageFileType())
            return new Photo(MediaPath, fileInfo);

        // shouldn't occur, but handle it anyway...
        return new File(fsInfo.Name, itemPath);
    }
}
