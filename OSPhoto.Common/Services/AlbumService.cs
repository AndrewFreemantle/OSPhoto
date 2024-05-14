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

namespace OSPhoto.Common.Services;

public class AlbumService(ApplicationDbContext dbContext, ILogger<AlbumService> logger) : IAlbumService
{
    private string MediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

    public AlbumResult Get(string id = "")
    {
        try
        {
            var path = string.IsNullOrEmpty(id)
                ? MediaPath
                : Path.Join(MediaPath, ItemBase.GetPathFromId(id, Album.IdPrefix));

            return new AlbumResult(GetContentDirectory(path)
                .EnumerateFileSystemInfos()
                .Where(fsi => fsi is DirectoryInfo || ((FileInfo)fsi).IsImageFileType())
                .Select(fsi => ItemBase.ConvertToItemBase(fsi, MediaPath, dbContext))
                .OrderByDescending(item => item.GetType() == typeof(Album))
                .ThenBy(item => item.Name), path, MediaPath);
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
                ? MediaPath
                : Path.Join(MediaPath, ItemBase.GetPathFromId(parentAlbumId, Album.IdPrefix));

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
        // is this an album thumbnail request?
        if (id.StartsWith(Album.IdPrefix))
        {
            var albumRecord = await dbContext.Albums.FindAsync(id);
            if (albumRecord != null && !string.IsNullOrEmpty(albumRecord.CoverPhotoId))
            {
                id = albumRecord.CoverPhotoId;
            }
            else
            {
                logger.LogWarning("GetThumbnail: No cover photo id found for album: {albumId}", id);
                throw new ArgumentException("No cover photo set for album");
            }
        }

        // grab the file, then return a resized version
        var memoryStream = new MemoryStream();

        var imagePath = Path.Combine(MediaPath, ItemBase.GetPathFromId(id, Photo.IdPrefix));

        using (var image = SixLabors.ImageSharp.Image.Load(imagePath))
        {
            image.Mutate(x => x.Resize(ThumbnailInfo.ThumbnailWidthInPixels, 0));
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
                await dbContext.Albums.AddAsync(new Database.Models.Album(id, ItemBase.GetPathFromId(id, Album.IdPrefix), null, null, photoId));
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

    public async Task<bool> Edit(string id, string? title, string? description)
    {
        try
        {
            var albumRecord = await dbContext.Albums.FindAsync(id);
            if (albumRecord == null)
                await dbContext.Albums.AddAsync(new Database.Models.Album(id, ItemBase.GetPathFromId(id, Album.IdPrefix), title, description));
            else
            {
                albumRecord.Title = title;
                albumRecord.Description = description;
                albumRecord.UpdatedDate = DateTime.UtcNow;
            }

            await dbContext.SaveChangesAsync();

            return true;
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception editing Album info. Album: {albumId} (title: {title}; desc: {description}",
                id,
                title,
                description);

            return false;
        }
    }

    public async Task<bool> Delete(string id)
    {
        var albumPath = string.IsNullOrEmpty(id)
            ? MediaPath
            : Path.Join(MediaPath, ItemBase.GetPathFromId(id, Album.IdPrefix));

        // shouldn't be possible to select or delete the MediaPath root, but prevent it happening anyway
        if (albumPath == MediaPath)
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

    private DirectoryInfo GetContentDirectory(string path = null)
    {
        return new DirectoryInfo(
            string.IsNullOrEmpty(path)
            || string.IsNullOrWhiteSpace(path)
                ? MediaPath
                : Path.Combine(MediaPath, path));
    }
}
