using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Database.Models;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using File = System.IO.File;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;
using Photo = OSPhoto.Common.Models.Photo;

namespace OSPhoto.Common.Services;

public class ImportService(ApplicationDbContext dbContext, ILogger<ImportService> logger) : IImportService
{
    private string _synoSharePathPrefix = Environment.GetEnvironmentVariable("SYNO_SHARE_PATH_PREFIX") ?? "/volume1/photo/";

    private string _importPath = Environment.GetEnvironmentVariable("IMPORT_PATH");

    private string _importSuccessPath = Path.Join(Environment.GetEnvironmentVariable("IMPORT_PATH"), "success");
    private string _importFailedPath = Path.Join(Environment.GetEnvironmentVariable("IMPORT_PATH"), "failed");

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
                MoveImportFile(photoImageFilename, _importSuccessPath);
            else
                MoveImportFile(photoImageFilename, _importFailedPath);

        var photoShareFilename = files.FirstOrDefault(fn => fn.Contains("photo_share"));
        if (!string.IsNullOrEmpty(photoShareFilename))
            await ImportPhotoShare(photoShareFilename);
    }

    private void MoveImportFile(string photoImageFilename, string destination)
    {
        try
        {
            // ensure the destination exists
            Directory.CreateDirectory(destination);

            var file = new FileInfo(photoImageFilename);
            file = new FileInfo(Path.Join(destination, file.Name));

            // check destination doesn't have a file with the same name already...
            if (file.Exists)
                file = new FileInfo(Path.Join(destination, $"{DateTime.UtcNow.Ticks}-{file.Name}"));

            File.Move(photoImageFilename, file.FullName);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Exception moving the imported file");
        }
    }

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
            record.Title,
            record.Description));

        return true;
    }

    private async Task<bool> ImportNotFound(CsvPhotoImageRecord record, string csvFilename)
    {
        var pfnf = await dbContext.PhotosFileNotFound.FindAsync(record.Id);
        if (pfnf != null) return false;

        await dbContext.PhotosFileNotFound.AddAsync(new PhotoFileNotFound(record, csvFilename));

        return true;
    }

    private async Task ImportPhotoShare(string photoShareFilename)
    {
        logger.LogError("Photo Share info import NOT YET IMPLEMENTED");
    }
}

public class CsvPhotoImageRecord
{
    [Name("id")]
    public int Id { get; set; }
    [Name("path")]
    public string Path { get; set; }
    [Name("title")]
    public string? Title { get; set; }
    [Name("description")]
    public string? Description { get; set; }

    [Name("camera_make")]
    public string? CameraMake { get; set; }
    [Name("camera_model")]
    public string? CameraModel { get; set; }
    [Name("timetaken")]
    public string? TimeTaken { get; set; }
    [Name("shareid")]
    public int? ShareId { get; set; }
}
