using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Exceptions;
using OSPhoto.Common.Extensions;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using Exception = System.Exception;
using OSPhoto.Common.Configuration;
using Microsoft.Extensions.Options;

namespace OSPhoto.Common.Services;

public class AlbumService : IAlbumService
{
    private readonly ApplicationDbContext dbContext;
    private readonly IFileSystem fileSystem;
    private readonly IOptions<AppSettings> settings;
    private readonly ILogger<AlbumService> logger;

    public AlbumService(ApplicationDbContext dbContext, IFileSystem fileSystem, IOptions<AppSettings> settings, ILogger<AlbumService> logger)
    {
        this.dbContext = dbContext;
        this.fileSystem = fileSystem;
        this.settings = settings;
        this.logger = logger;
    }

    public string MediaPath => settings.Value.MediaPath;

    public AlbumResult Get(string id = "")
    {
        try
        {
            var path = string.IsNullOrEmpty(id)
                ? settings.Value.MediaPath
                : Path.Join(settings.Value.MediaPath, ItemBase.GetPathFromId(id));

            return new AlbumResult(GetContentDirectory(path)
                .EnumerateFileSystemInfos()
                .Where(fsi => fsi is IDirectoryInfo || (fsi is IFileInfo fileInfo && (fileInfo.IsImageFileType() || fileInfo.IsVideoFileType())))
                .Select(fsi => ItemBase.ConvertToItemBase(fsi, settings.Value.MediaPath, dbContext))
                .OrderByDescending(item => item.GetType() == typeof(Album))
                .ThenBy(item => item.Name), path, settings.Value.MediaPath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception getting album (directory) contents for {id}", id);
            throw new AlbumServiceException(id, e.Message, e);
        }
    }

    public Task Create(string parentAlbumId, string albumName)
    {
        try
        {
            var parentPath = string.IsNullOrEmpty(parentAlbumId)
                ? settings.Value.MediaPath
                : Path.Join(settings.Value.MediaPath, ItemBase.GetPathFromId(parentAlbumId));

            Directory.CreateDirectory(Path.Join(parentPath, albumName));
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error creating new album '{name}' (in id: {parentAlbumId})"
                , albumName,
                parentAlbumId);
        }

        return Task.CompletedTask;
    }

    public async Task<Stream> GetThumbnail(string id)
    {
        string? imagePath = null;

        // is this an album thumbnail request?
        if (!id.StartsWith(Album.IdPrefix))
            return await Task.FromResult(Stream.Null);

        var albumRecord = await dbContext.Albums.FindAsync(id);
        if (albumRecord != null && !string.IsNullOrEmpty(albumRecord.CoverPhotoId))
        {
            id = albumRecord.CoverPhotoId;
        }
        else
        {
            // return the photo id of the last image file within the album/directory
            var albumPath = Path.Join(settings.Value.MediaPath, ItemBase.GetPathFromId(id));
            var dirInfo = fileSystem.DirectoryInfo.New(albumPath);

            var lastImageFileInfo = dirInfo
                .EnumerateFiles()
                .Where(fi => fi.IsImageFileType())
                .MaxBy(fi => fi.Name);

            if (lastImageFileInfo != null)
                imagePath = lastImageFileInfo.FullName;
            else
            {
                logger.LogWarning("GetThumbnail: No cover photo id found or images contained in album: {albumId}", id);
                throw new ArgumentException("No cover photo set or images contained in album");
            }
        }

        // grab the file, then return a resized version
        var memoryStream = new MemoryStream();

        imagePath ??= Path.Combine(settings.Value.MediaPath, ItemBase.GetPathFromId(id));

        using (var image = SixLabors.ImageSharp.Image.Load(imagePath))
        {
            image.Mutate(x => x.Resize(settings.Value.ThumbnailWidthInPixels, 0));
            image.Save(memoryStream, new JpegEncoder());
            memoryStream.Position = 0;
            return memoryStream;
        }
    }

    public async Task<bool> SetCoverPhoto(string id, string photoId)
    {
        try
        {
            var albumRecord = await dbContext.Albums.FindAsync(id);
            if (albumRecord == null)
                await dbContext.Albums.AddAsync(new Database.Models.Album(
                    id,
                    Path.Join(settings.Value.MediaPath, ItemBase.GetPathFromId(id)),
                    null,
                    null,
                    photoId));
            else
            {
                albumRecord.CoverPhotoId = photoId;
                albumRecord.UpdatedDate = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception setting album cover photo. Album: {albumId}, Photo: {photoId}",
                id,
                photoId);

            return false;
        }
    }

    public async Task<bool> Edit(string id, string? title, string? description, string? coverPhotoId)
    {
        try
        {
            var albumRecord = await dbContext.Albums.FindAsync(id);
            if (albumRecord == null)
                await dbContext.Albums.AddAsync(new Database.Models.Album(
                    id,
                    Path.Join(settings.Value.MediaPath, ItemBase.GetPathFromId(id)),
                    title,
                    description,
                    coverPhotoId));
            else
            {

                albumRecord.Title = title;
                albumRecord.Description = description;
                albumRecord.CoverPhotoId = coverPhotoId;
                albumRecord.UpdatedDate = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception editing Album info. Album: {albumId} (title: {title}; desc: {description}, coverPhotoId: {coverPhotoId})",
                id,
                title,
                description,
                coverPhotoId);

            return false;
        }
    }

    public async Task<bool> Delete(string id)
    {
        var albumPath = string.IsNullOrEmpty(id)
            ? settings.Value.MediaPath
            : Path.Join(settings.Value.MediaPath, ItemBase.GetPathFromId(id));

        // shouldn't be possible to select or delete the MediaPath root, but prevent it happening anyway
        if (albumPath == settings.Value.MediaPath)
            return false;

        try
        {
            Directory.Delete(albumPath, true);

            var albumMetadataDeleted = await dbContext.Albums
                .Where(a => a.Path.StartsWith(albumPath))
                .ExecuteDeleteAsync();

            var photoMetadataDeleted = await dbContext.Photos
                .Where(p => p.Path.StartsWith(albumPath))
                .ExecuteDeleteAsync();

            await dbContext.SaveChangesAsync();

            logger.LogInformation("Deleted album '{albumPath}', {albumMetadataCount} album & {photoMetadataCount} photo metadata records",
                albumPath,
                albumMetadataDeleted,
                photoMetadataDeleted);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting album id: {id}", id);
            return false;
        }

        return true;
    }

    private IDirectoryInfo GetContentDirectory(string path = null)
    {
        return fileSystem.DirectoryInfo.New(
            string.IsNullOrEmpty(path)
            || string.IsNullOrWhiteSpace(path)
                ? settings.Value.MediaPath
                : Path.Combine(settings.Value.MediaPath, path));
    }
}
