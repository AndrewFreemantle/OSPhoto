using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using OSPhoto.Common.Configuration;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using OSPhoto.Common.Services.Models;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;

namespace OSPhoto.Common.Services;

public class PhotoService(ApplicationDbContext dbContext, ICommentService commentService, IFileSystem fileSystem, IOptions<AppSettings> settings, ILogger<PhotoService> logger) : ServiceBase(dbContext, fileSystem, settings, logger), IPhotoService
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
            image.Mutate(x => x.Resize(settings.Value.ThumbnailWidthInPixels, 0));
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
        var fullDestination = await CheckIfDestinationExists(Path.Combine(destination, fileName));

        this.logger.LogInformation("Writing new file to: {fullDestination}", fullDestination);
        try
        {
            using (var stream = new FileStream(fullDestination, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
        }
        catch (Exception e)
        {
            this.logger.LogError(e, "Error uploading file to {fullDestination}", fullDestination);
            return false;
        }

        // TODO: Pull this workaround into a plugin, once we have plugin support
        #region iOS bug workaround
            //  iOS imported geo-tagged photos bug workaround... if a geotagged photo is imported into iOS, iOS assumes that
            //      no timezone on the Date Taken means the media was created in the Geo-Located timezone and imported into
            //      into the timezone of the iOS.
            //      This is often incorrect as the camera used to take the photo had the date and time already set correctly
            //      for the timezone the photo was taken in.
            //      While the Date Taken and EXIF metadata are correct, the filename iOS generates can be off by hours.
            //      This is a quick workaround to ensure the filename matches the date taken.

        if (Regex.IsMatch(fileName, @"^IMG_\d{8}_\d{6}\.JPG$"))
        {
            try
            {
                // correct format, now what should it be?
                var photo = GetInfo(ItemBase.GetIdForPath(_mediaPath, fileSystem.FileInfo.New(fullDestination), Photo.IdPrefix));
                var fileNameBasedOnTakenDate = $"IMG_{photo.Info.TakenDate.Replace("-", "").Replace(":", "").Replace(" ", "_")}.JPG";
                if (!fileName.Equals(fileNameBasedOnTakenDate, StringComparison.OrdinalIgnoreCase)
                    && photo.Additional.PhotoExif.Camera != null
                    && photo.Additional.PhotoExif.Camera.Contains("OLYMPUS"))
                {
                    // rename / move the file
                    this.logger.LogInformation("iOS imported-geotagged filename bug spotted - renaming file from {fileName} to {fileNameBasedOnTakenDate}", fileName, fileNameBasedOnTakenDate);

                    // add in the new filename and check if it exists
                    var fullDestinationBasedOnTakenDate = await CheckIfDestinationExists(Path.Combine(destination, fileNameBasedOnTakenDate));

                    try
                    {
                        fileSystem.File.Move(fullDestination, fullDestinationBasedOnTakenDate);
                        fullDestination = fullDestinationBasedOnTakenDate;
                    }
                    catch (Exception innerE)
                    {
                        this.logger.LogError(innerE, "Error renaming file to {fullDestinationBasedOnTakenDate}", fullDestinationBasedOnTakenDate);
                    }
                }
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "iOS imported-geotagged filename bug fix error. File not renamed");
            }
        }
        #endregion

        // any metadata to save?
        if (!string.IsNullOrEmpty(title) || !string.IsNullOrEmpty(description))
        {
            try
            {
                this.logger.LogInformation("Writing new file title/desc to database");
                title = title ?? fileName;
                description = description ?? null;

                var id = ItemBase.GetIdForPath(_mediaPath, fileSystem.FileInfo.New(fullDestination), Photo.IdPrefix);
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
                this.logger.LogWarning(e, "Error saving title and description for {fullDestination}", fullDestination);
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
