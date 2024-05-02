using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;

namespace OSPhoto.Common.Services;

public class PhotoService(ApplicationDbContext dbContext, ILogger<PhotoService> logger) : IPhotoService
{
    private string MediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

    public Photo GetInfo(string id)
    {
        var photoPath = Path.Combine(MediaPath, ItemBase.GetPathFromId(id, Photo.IdPrefix));
        return ItemBase.ConvertToItemBase(new FileInfo(photoPath), MediaPath, dbContext) as Photo;
    }

    public async Task EditInfo(string id, string title, string description)
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

    public async Task<bool> Upload(IFormFile file, string destinationAlbum, string fileName, string? title, string? description)
    {
        // where do we need to upload it to?
        var destination = MediaPath;
        if (!string.IsNullOrEmpty(destinationAlbum))
            destination = Path.Combine(destination, destinationAlbum);

        destination = Path.Combine(destination, fileName);

        logger.LogInformation("Writing new file to: {destination}", destination);
        try
        {
            using (var stream = new FileStream(destination, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error uploading file to {destination}", destination);
            return false;
        }

        // any metadata to save?
        if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(description))
        {
            try
            {
                logger.LogInformation("Writing new file title/desc to database");
                title = title ?? fileName;
                description = description ?? null;

                var id = ItemBase.GetIdForPath(MediaPath, new FileInfo(destination), Photo.IdPrefix);
                var photo = await dbContext.Photos.FindAsync(id);
                if (photo == null)
                    await dbContext.Photos.AddAsync(new DbPhoto(id, MediaPath, title, description));
                else
                {
                    photo.Title = title;
                    photo.Description = description;
                    photo.UpdatedDate = DateTime.UtcNow;
                    photo.ImportedShareId = null;
                }

                await dbContext.SaveChangesAsync();
            }
            catch (Exception e)
            {
                logger.LogWarning(e, "Error saving title and description for {destination}", destination);
            }
        }

        return true;
    }

    public async Task<bool> Move(string id, string destinationAlbumId, bool isOverwrite)
    {
        var imagePath = Path.Combine(MediaPath, ItemBase.GetPathFromId(id, Photo.IdPrefix));

        var destinationPath = Path
            .Join(
                MediaPath,
                ItemBase.GetPathFromId(destinationAlbumId, Album.IdPrefix),
                Path.GetFileName(imagePath));

        try
        {
            if (!System.IO.File.Exists(destinationPath) || (System.IO.File.Exists(destinationPath) && isOverwrite))
            {
                System.IO.File.Move(imagePath, destinationPath);

                // is there any metadata that we need to move as well?
                var photoRecord = await dbContext.Photos.FindAsync(id);
                if (photoRecord != null)
                {

                    await dbContext.Photos.AddAsync(new DbPhoto(
                        ItemBase.GetIdForPath(MediaPath, new FileInfo(destinationPath), Photo.IdPrefix),
                        destinationPath,
                        photoRecord.Title,
                        photoRecord.Description,
                        photoRecord.ImportedShareId));

                    dbContext.Photos.Remove(photoRecord);

                    await dbContext.SaveChangesAsync();
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error moving photo (id: {id}) from {src} to {dest}",
                id,
                imagePath,
                destinationPath);

            return false;
        }

        return true;
    }

    public async Task Delete(string id)
    {
        var imagePath = Path.Combine(MediaPath, ItemBase.GetPathFromId(id, Photo.IdPrefix));

        try
        {
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting photo image: {path}", imagePath);
        }

        try
        {
            await dbContext
                .Photos
                .Where(p => p.Id == id)
                .ExecuteDeleteAsync();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error deleting photo metadata for id: {id}", id);
        }
    }
}
