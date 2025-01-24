using Microsoft.AspNetCore.Http;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using OSPhoto.Common.Services.Models;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;

namespace OSPhoto.Common.Services;

public class PhotoService(ApplicationDbContext dbContext, ICommentService commentService, IFileSystem fileSystem, ILogger<PhotoService> logger) : ServiceBase(dbContext, fileSystem, logger), IPhotoService
{
    public async Task<Stream> GetThumbnail(string id)
    {
        if (!id.StartsWith(Photo.IdPrefix))
            return await Task.FromResult(Stream.Null);

        // grab the file, then return a resized version
        var memoryStream = new MemoryStream();
        var imagePath = Path.Combine(_mediaPath, ItemBase.GetPathFromId(id));

        using (var image = SixLabors.ImageSharp.Image.Load(imagePath))
        {
            image.Mutate(x => x.Resize(ThumbnailInfo.ThumbnailWidthInPixels, 0));
            image.Save(memoryStream, new JpegEncoder());
            memoryStream.Position = 0;
            return memoryStream;
        }
    }

    public new async Task<MoveResult> Move(string id, string destinationAlbumId, bool isOverwrite)
    {
        // do everything in the base method...
        var result = await base.Move(id, destinationAlbumId, isOverwrite);

        // then move any comments
        // TODO: use Mediatr / pub/sub instead of coupling these services
        if (result.Success)
            result = await commentService.Move(id, result.NewId!);

        return result;
    }

    public new async Task Delete(string id)
    {
        // do everything in the base method...
        await base.Delete(id);

        // then delete any comments
        // TODO: use Mediatr / pub/sub instead of coupling these services
        await commentService.Delete(id);

        return;
    }

    public async Task<bool> Upload(IFormFile file, string destinationAlbum, string fileName, string? title, string? description)
    {
        // where do we need to upload it to?
        var destination = _mediaPath;
        if (!string.IsNullOrEmpty(destinationAlbum))
            destination = Path.Combine(destination, destinationAlbum);

        // add in the fileName and check if it exists
        destination = await CheckIfDestinationExists(Path.Combine(destination, fileName));

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

                var id = ItemBase.GetIdForPath(_mediaPath, fileSystem.FileInfo.New(destination), Photo.IdPrefix);
                var photo = await dbContext.Photos.FindAsync(id);
                if (photo == null)
                    await dbContext.Photos.AddAsync(new DbPhoto(id, _mediaPath, title, description));
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

    private const char SuffixSeparator = '_';

    public async Task<string> CheckIfDestinationExists(string destination)
    {
        var fi = fileSystem.FileInfo.New(destination);
        if (!fi.Exists) return destination;

        var filename = fi.Name.Substring(0, fi.Name.Length - fi.Extension.Length);
        var filenameExtension = fi.Extension;
        var suffix = 1;

        do
        {
            destination = Path.Combine(fi.DirectoryName, $"{filename}{SuffixSeparator}{suffix}{filenameExtension}");
            suffix++;
        } while (fileSystem.File.Exists(destination));

        // add a suffix
        return await Task.FromResult(destination);
    }
}
