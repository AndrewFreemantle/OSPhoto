using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Database.Models;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using OSPhoto.Common.Services.Models;
using File = System.IO.File;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;
using Photo = OSPhoto.Common.Models.Photo;
using DbAlbum = OSPhoto.Common.Database.Models.Album;
using Album = OSPhoto.Common.Models.Album;

namespace OSPhoto.Common.Services;

public class ImportService(ApplicationDbContext dbContext, ILogger<ImportService> logger) : IImportService
{
    private string _synoSharePathPrefix = Environment.GetEnvironmentVariable("SYNO_SHARE_PATH_PREFIX") ?? "/volume1/photo/";

    private readonly string _importPath = Environment.GetEnvironmentVariable("IMPORT_PATH");

    private readonly string _importPathSuccess = Path.Join(Environment.GetEnvironmentVariable("IMPORT_PATH"), "success");
    private readonly string _importPathFailed = Path.Join(Environment.GetEnvironmentVariable("IMPORT_PATH"), "failed");

    public async Task RunAsync()
    {
        if (!Directory.Exists(_importPath) || !Directory.GetFiles(_importPath).Any())
        {
            logger.LogInformation(@"Import directory doesn't exist or is empty, no photo descriptions to import.
Looked in: {importPath}", _importPath);
            return;
        }

        var files = Directory.GetFiles(_importPath);
        var photoImageFilename = files.FirstOrDefault(fn => fn.Contains("photo_image"));
        if (!string.IsNullOrEmpty(photoImageFilename))
            if (await ImportPhotoImage(photoImageFilename))
                MoveImportFile(photoImageFilename, _importPathSuccess);
            else
                MoveImportFile(photoImageFilename, _importPathFailed);

        var photoShareFilename = files.FirstOrDefault(fn => fn.Contains("photo_share"));
        if (!string.IsNullOrEmpty(photoShareFilename))
            if (await ImportPhotoShare(photoShareFilename))
                MoveImportFile(photoShareFilename, _importPathSuccess);
            else
                MoveImportFile(photoShareFilename, _importPathFailed);
    }

    private void MoveImportFile(string importFilename, string destination)
    {
        try
        {
            // ensure the destination exists
            Directory.CreateDirectory(destination);

            var file = new FileInfo(importFilename);
            file = new FileInfo(Path.Join(destination, file.Name));

            // check destination doesn't have a file with the same name already...
            if (file.Exists)
                file = new FileInfo(Path.Join(destination, $"{DateTime.UtcNow.Ticks}-{file.Name}"));

            File.Move(importFilename, file.FullName);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception moving the imported file");
        }
    }

    /// <summary>
    /// Imports records exported from the `photo_image` database table into OSPhoto
    /// </summary>
    /// <param name="csvFilename"></param>
    /// <returns></returns>
    private async Task<bool> ImportPhotoImage(string csvFilename)
    {
        logger.LogInformation("Starting import of: {file}", csvFilename);

        var mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

        var recordsImported = 0;
        var recordsNotFound = 0;

        try
        {
            using (var reader = new StreamReader(csvFilename))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                await csv.ReadAsync();
                csv.ReadHeader();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var record = csv.GetRecord<CsvPhotoImageRecord>();

                        if (!IsImportable(record))
                            continue;

                        // trim the start of the path to match our config
                        var recordPath = record.Path;
                        if (recordPath.StartsWith(_synoSharePathPrefix))
                            recordPath = Path.Join(mediaPath, recordPath.Substring(_synoSharePathPrefix.Length));

                        if (File.Exists(recordPath))
                        {
                            var fsInfo = new FileInfo(recordPath);
                            record.Title ??= fsInfo.Name;
                            if (await Import(ItemBase.GetIdForPath(mediaPath, fsInfo, Photo.IdPrefix), record))
                                recordsImported++;
                        }
                        else
                        {
                            logger.LogDebug(" > Photo file not found: {recordPath}", recordPath);
                            if (await ImportNotFound(record, csvFilename))
                                recordsNotFound++;
                        }
                    }
                    catch (Exception innerE)
                    {
                        logger.LogError(innerE, "Exception importing CSV record");
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception reading import file: {file}", csvFilename);
            return false;
        }
        finally
        {
            await dbContext.SaveChangesAsync();
        }

        logger.LogInformation("Finished. Imported {recordsImportedCount} successfully from: {file}", recordsImported, csvFilename);

        if (recordsNotFound > 0)
            logger.LogInformation(" > Saved information about {recordsFileNotFoundCount} photos that couldn't find the image file for.", recordsNotFound);

        return true;
    }

    /// <summary>
    /// Checks if the CSV record has the minimum information needed to be usefully imported
    /// </summary>
    private bool IsImportable(CsvPhotoImageRecord record)
    {
        return !string.IsNullOrEmpty(record.Path)
               && !string.IsNullOrWhiteSpace(record.Path)
               && !string.IsNullOrEmpty(record.Description)
               && !string.IsNullOrWhiteSpace(record.Description);
    }

    private async Task<bool> Import(string id, CsvPhotoImageRecord record)
    {
        var photo = await dbContext.Photos.FindAsync(id);
        if (photo != null) return false;

        await dbContext.Photos.AddAsync(new DbPhoto(
            id,
            record.Path,
            record.Title!,
            record.Description!,
            record.ShareId));

        return true;
    }

    private async Task<bool> ImportNotFound(CsvPhotoImageRecord record, string csvFilename)
    {
        var pfnf = await dbContext.PhotosFileNotFound.FindAsync(record.Id);
        if (pfnf != null) return false;

        await dbContext.PhotosFileNotFound.AddAsync(new PhotoFileNotFound(record, csvFilename));

        return true;
    }

    /// <summary>
    /// Imports records exported from the `photo_share` database table into OSPhoto
    /// </summary>
    /// <param name="csvFilename"></param>
    private async Task<bool> ImportPhotoShare(string csvFilename)
    {
        logger.LogInformation("Starting import of: {file}", csvFilename);

        var mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

        var recordsImported = 0;
        var recordsNotFound = 0;

        try
        {
            using (var reader = new StreamReader(csvFilename))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                await csv.ReadAsync();
                csv.ReadHeader();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var record = csv.GetRecord<CsvPhotoShareRecord>();

                        // the sharename isn't the full path
                        var recordPath = Path.Join(mediaPath, record.ShareName);

                        if (Directory.Exists(recordPath))
                        {
                            var dirInfo = new DirectoryInfo(recordPath);
                            record.Title ??= dirInfo.Name;
                            if (await Import(ItemBase.GetIdForPath(mediaPath, dirInfo, Album.IdPrefix), record))
                                recordsImported++;
                        }
                        else
                        {
                            logger.LogDebug(" > Album (directory) not found: {recordPath}", record.ShareName);
                            if (await ImportNotFound(record, csvFilename))
                                recordsNotFound++;
                        }
                    }
                    catch (Exception innerE)
                    {
                        logger.LogError(innerE, "Exception importing CSV record");
                    }
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception reading import file: {file}", csvFilename);
            return false;
        }
        finally
        {
            await dbContext.SaveChangesAsync();
        }

        logger.LogInformation("Finished. Imported {recordsImportedCount} successfully from: {file}", recordsImported, csvFilename);

        if (recordsNotFound > 0)
            logger.LogInformation(" > Saved information about {recordsFileNotFoundCount} albums that couldn't find the directory for.", recordsNotFound);

        return true;
    }

    private async Task<bool> Import(string id, CsvPhotoShareRecord record)
    {
        var album = await dbContext.Albums.FindAsync(id);
        if (album != null) return false;

        // find the photo for this album's cover
        var coverPhoto = await dbContext.Photos.FirstOrDefaultAsync(p => p.ImportedShareId == record.Id);
        if (coverPhoto == null)
        {
            logger.LogDebug(" > Can't find photo image for album/shareid: {shareid} ({shareName})",
                record.Id,
                record.ShareName);

            return false;
        }

        await dbContext.Albums.AddAsync(new DbAlbum(
            id,
            record.ShareName,
            record.Title!,
            record.Description!,
            coverPhoto.Id));

        return true;
    }

    private async Task<bool> ImportNotFound(CsvPhotoShareRecord record, string csvFilename)
    {
        var adnf = await dbContext.AlbumsDirNotFound.FindAsync(record.Id);
        if (adnf != null) return false;

        await dbContext.AlbumsDirNotFound.AddAsync(new AlbumDirNotFound(record, csvFilename));

        return true;
    }
}
