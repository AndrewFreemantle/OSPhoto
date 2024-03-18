using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using Microsoft.Extensions.Logging;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using File = System.IO.File;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;

namespace OSPhoto.Common.Services;

public class ImportService(ApplicationDbContext dbContext, ILogger<ImportService> logger) : IImportService
{
    private string _synoSharePathPrefix = Environment.GetEnvironmentVariable("SYNO_SHARE_PATH_PREFIX") ?? "/volume1/photo/";

    private string _importPath = Environment.GetEnvironmentVariable("IMPORT_PATH");

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
            await ImportPhotoImage(photoImageFilename);

        var photoShareFilename = files.FirstOrDefault(fn => fn.Contains("photo_share"));
        if (!string.IsNullOrEmpty(photoShareFilename))
            await ImportPhotoShare(photoShareFilename);
    }

    private async Task ImportPhotoImage(string csvFilename)
    {
        logger.LogInformation("Starting import of: {file}", csvFilename);

        var mediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH");

        int recordsImported = 0;

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
                        var record = csv.GetRecord<CsvPhotoImage>();

                        if (!string.IsNullOrEmpty(record.Path)
                            && !string.IsNullOrEmpty(record.Description)
                            && !string.IsNullOrWhiteSpace(record.Description))
                        {
                            // trim the start of the path to match our config
                            if (record.Path.StartsWith(_synoSharePathPrefix))
                                record.Path = Path.Join(mediaPath, record.Path.Substring(_synoSharePathPrefix.Length));

                            if (!File.Exists(record.Path))
                                continue;

                            var fsInfo = new FileInfo(record.Path);
                            record.Title ??= fsInfo.Name;
                            if (await Import(ItemBase.GetIdForPath(mediaPath, fsInfo, Photo.IdPrefix), record));
                                recordsImported++;
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
        }
        finally
        {
            await dbContext.SaveChangesAsync();
        }

        if (recordsImported > 0)
            logger.LogInformation("Finished. Imported {recordCount} from: {file}", recordsImported, csvFilename);
        else
            logger.LogInformation("Finished. Nothing imported from {file}", csvFilename);
    }

    private async Task<bool> Import(string id, CsvPhotoImage record)
    {
        var photo = await dbContext.Photos.FindAsync(id);
        if (photo == null)
        {
            await dbContext.Photos.AddAsync(new DbPhoto(
                id,
                record.Path,
                record.Title,
                record.Description));

            return true;
        }

        return false;
    }

    private async Task ImportPhotoShare(string photoShareFilename)
    {
        logger.LogError("Photo Share info import NOT YET IMPLEMENTED");
    }
}

public class CsvPhotoImage
{
    [Name("path")]
    public string Path { get; set; }
    [Name("title")]
    public string? Title { get; set; }
    [Name("description")]
    public string? Description { get; set; }
}
