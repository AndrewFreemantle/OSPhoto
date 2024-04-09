using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Exceptions;
using OSPhoto.Common.Extensions;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using File = OSPhoto.Common.Models.File;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;

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
                : Path.Combine(MediaPath, id[Album.IdPrefix.Length..].FromHex());

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

    public async Task<bool> SetCoverPhoto(string albumId, string photoId)
    {
        try
        {
            var albumRecord = await dbContext.Albums.FindAsync(albumId);
            if (albumRecord == null)
                await dbContext.Albums.AddAsync(new Database.Models.Album(albumId, ItemBase.GetPathFromId(albumId, Album.IdPrefix), null, null, photoId));
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
                albumId,
                photoId);

            return false;
        }
    }

    public async Task<bool> Edit(string albumId, string? title, string? description)
    {
        try
        {
            var albumRecord = await dbContext.Albums.FindAsync(albumId);
            if (albumRecord == null)
                await dbContext.Albums.AddAsync(new Database.Models.Album(albumId, ItemBase.GetPathFromId(albumId, Album.IdPrefix), title, description));
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
                albumId,
                title,
                description);

            return false;
        }
    }

    public Photo GetPhoto(string id)
    {
        var photoPath = Path.Combine(MediaPath, ItemBase.GetPathFromId(id, Photo.IdPrefix));
        return ConvertToItemBase(new FileInfo(photoPath)) as Photo;
    }

    public async Task EditPhoto(string id, string title, string description)
    {
        try
        {
            var photo = await dbContext.Photos.FindAsync(id);
            if (photo == null)
            {
                await dbContext.Photos.AddAsync(new DbPhoto(
                    id,
                    Path.Combine(MediaPath, ItemBase.GetPathFromId(id, Photo.IdPrefix)),
                    title,
                    description));
            }
            else
            {
                photo.Title = title;
                photo.Description = description;
            }

            await dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception editing photo metadata for {id}; given {title}, {description}", id, title, description);
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
            return new Album(MediaPath, directoryInfo, dbContext);

        if (fsInfo is FileInfo fileInfo && fileInfo.IsImageFileType())
            return new Photo(MediaPath, fileInfo, dbContext);

        // shouldn't occur, but handle it anyway...
        return new File(fsInfo.Name, itemPath);
    }
}
