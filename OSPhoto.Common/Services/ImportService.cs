using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using OSPhoto.Common.Database;
using OSPhoto.Common.Database.Models;
using OSPhoto.Common.Extensions;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;
using OSPhoto.Common.Services.Models;
using File = System.IO.File;
using DbPhoto = OSPhoto.Common.Database.Models.Photo;
using Photo = OSPhoto.Common.Models.Photo;
using DbAlbum = OSPhoto.Common.Database.Models.Album;
using Album = OSPhoto.Common.Models.Album;
using DbComment = OSPhoto.Common.Database.Models.Comment;
using OSPhoto.Common.Configuration;
using Microsoft.Extensions.Options;

namespace OSPhoto.Common.Services;

public class ImportService : IImportService
{
    private readonly string _synoSharePathPrefix;
    private readonly string _mediaPath;
    private readonly string _importPath;
    private readonly string _importPathSuccess;
    private readonly string _importPathFailed;
    private readonly string _importDateTimeFormat = "yyyy-MM-dd HH:mm:ss";
    private readonly string? _importTimezoneCulture;
    private readonly IPhotoService photoService;
    private readonly IAlbumService albumService;
    private readonly ApplicationDbContext dbContext;
    private readonly IFileSystem fileSystem;
    private readonly ILogger<ImportService> logger;

    public ImportService(IPhotoService photoService, IAlbumService albumService, ApplicationDbContext dbContext, IFileSystem fileSystem,  IOptions<AppSettings> settings, ILogger<ImportService> logger)
    {
        this.photoService = photoService;
        this.albumService = albumService;
        this.dbContext = dbContext;
        this.fileSystem = fileSystem;
        this.logger = logger;
        _synoSharePathPrefix = settings.Value.SynoSharePathPrefix;
        _mediaPath = settings.Value.MediaPath;
        _importPath = settings.Value.ImportPath;
        _importPathSuccess = Path.Join(settings.Value.ImportPath, "success");
        _importPathFailed = Path.Join(settings.Value.ImportPath, "failed");
        _importTimezoneCulture = settings.Value.ImportTimezoneCulture;
    }

    public async Task RunAsync()
    {
        logger.LogInformation("Starting...");

        if (!Directory.Exists(_importPath) || !Directory.GetFiles(_importPath).Any())
        {
            logger.LogInformation("Import directory doesn't exist or is empty, nothing to import.\nLooked in: {importPath}", _importPath);
            return;
        }

        var files = Directory.GetFiles(_importPath);
        var photoImageFilename = files.FirstOrDefault(fn => fn.Contains("photo_image"));
        if (!string.IsNullOrEmpty(photoImageFilename))
        {
            // we have photo metadata to import...
            if (await ImportPhotoImage(photoImageFilename))
                MoveImportFile(photoImageFilename, _importPathSuccess);
            else
                MoveImportFile(photoImageFilename, _importPathFailed);
        }
        else
        {
            // no photo metadata to import, but check if any we have for missing files now match...
            await CheckPhotoImagesFileNotFound();
        }

        var photoShareFilename = files.FirstOrDefault(fn => fn.Contains("photo_share"));
        if (!string.IsNullOrEmpty(photoShareFilename))
        {
            // we have photo share/album metadata to import...
            if (await ImportPhotoShare(photoShareFilename))
                MoveImportFile(photoShareFilename, _importPathSuccess);
            else
                MoveImportFile(photoShareFilename, _importPathFailed);
        }
        else
        {
            // no photo share/album metadata to import, but check if any we have for missing albums now match...
            await CheckAlbumsDirNotFound();
        }

        var photoCommentFilename = files.FirstOrDefault(fn => fn.Contains("photo_comment"));
        if (!string.IsNullOrEmpty(photoCommentFilename))
        {
            // we have photo comments to import...
            if (await ImportPhotoComment(photoCommentFilename))
                MoveImportFile(photoCommentFilename, _importPathSuccess);
            else
                MoveImportFile(photoCommentFilename, _importPathFailed);
        }
        else
        {
            // no photo comment metadata to import, but check if any we have for missing media now match...
            await CheckCommentsNotFound();
        }

        logger.LogInformation("Finished.");
    }

    private void MoveImportFile(string importFilename, string destination)
    {
        try
        {
            // ensure the destination exists
            Directory.CreateDirectory(destination);

            var file = fileSystem.FileInfo.New(importFilename);
            file = fileSystem.FileInfo.New(Path.Join(destination, file.Name));

            // check destination doesn't have a file with the same name already...
            if (file.Exists)
                file = fileSystem.FileInfo.New(Path.Join(destination, $"{DateTime.UtcNow.Ticks}-{file.Name}"));

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
                            recordPath = Path.Join(_mediaPath, recordPath.Substring(_synoSharePathPrefix.Length));

                        if (File.Exists(recordPath))
                        {
                            var fsInfo = fileSystem.FileInfo.New(recordPath);
                            record.Title ??= fsInfo.Name;
                            if (await Import(ItemBase.GetIdForPath(_mediaPath, fsInfo, Photo.IdPrefix), recordPath, record))
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

    private async Task<bool> Import(string id, string recordPath, CsvPhotoImageRecord record)
    {
        var photo = await dbContext.Photos.FindAsync(id);
        if (photo != null) return false;

        await dbContext.Photos.AddAsync(new DbPhoto(
            id,
            recordPath,
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
    /// Checks each record in the photo metadata database against the file system to see if there's now a matching file
    /// (this could be because we'd imported photo image metadata, but were still copying over the photo files)
    /// </summary>
    private async Task CheckPhotoImagesFileNotFound()
    {
        if (!await dbContext.PhotosFileNotFound.AnyAsync())
            return;

        try
        {
            for (int i = dbContext.PhotosFileNotFound.Count() - 1; i >= 0; i--)
            {
                try
                {
                    var record = await dbContext.PhotosFileNotFound.ElementAtAsync(i);

                    var recordPath = record.Path;
                    if (recordPath.StartsWith(_synoSharePathPrefix))
                        recordPath = Path.Join(_mediaPath, recordPath.Substring(_synoSharePathPrefix.Length));

                    if (File.Exists(recordPath))
                    {
                        using (var trans = await dbContext.Database.BeginTransactionAsync())
                        {
                            var fsInfo = fileSystem.FileInfo.New(recordPath);
                            var id = ItemBase.GetIdForPath(_mediaPath, fsInfo, Photo.IdPrefix);
                            await photoService.EditInfo(id, record.Title ?? fsInfo.Name, record.Description ?? string.Empty,
                                record.ShareId);

                            dbContext.PhotosFileNotFound.Remove(record);

                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception)
                {
                    // exception doesn't matter here as this record can be retried
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking photos not found metadata against the filesystem");
        }
    }

    /// <summary>
    /// Imports records exported from the `photo_share` database table into OSPhoto
    /// </summary>
    /// <param name="csvFilename"></param>
    private async Task<bool> ImportPhotoShare(string csvFilename)
    {
        logger.LogInformation("Starting import of: {file}", csvFilename);

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
                        var recordPath = Path.Join(_mediaPath, record.ShareName);

                        if (Directory.Exists(recordPath))
                        {
                            var dirInfo = fileSystem.DirectoryInfo.New(recordPath);
                            if (string.IsNullOrEmpty(record.Title))
                                record.Title = dirInfo.Name;

                            if (await Import(ItemBase.GetIdForPath(_mediaPath, dirInfo, Album.IdPrefix), record))
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

        // find the photo for this album's cover, if there is one
        var coverPhoto = await dbContext.Photos.FirstOrDefaultAsync(p => p.ImportedShareId == record.Id);

        await dbContext.Albums.AddAsync(new DbAlbum(
            id,
            Path.Join(_mediaPath, record.ShareName),
            record.Title!,
            record.Description!,
            coverPhoto?.Id));

        return true;
    }

    private async Task<bool> ImportNotFound(CsvPhotoShareRecord record, string csvFilename)
    {
        var adnf = await dbContext.AlbumsDirNotFound.FindAsync(record.Id);
        if (adnf != null) return false;

        await dbContext.AlbumsDirNotFound.AddAsync(new AlbumDirNotFound(record, csvFilename));

        return true;
    }

    /// <summary>
    /// Checks each record in the album metadata database against the file system to see if there's now a matching album directory
    /// (this could be because we'd imported album share metadata, but were still copying over the albums/directories)
    /// </summary>
    private async Task CheckAlbumsDirNotFound()
    {
        if (!await dbContext.AlbumsDirNotFound.AnyAsync())
            return;

        try
        {
            for (int i = dbContext.AlbumsDirNotFound.Count() - 1; i >= 0; i--)
            {
                try
                {
                    var record = await dbContext.AlbumsDirNotFound.ElementAtAsync(i);

                    // the sharename isn't the full path
                    var recordPath = Path.Join(_mediaPath, record.ShareName);

                    if (Directory.Exists(recordPath))
                    {
                        using (var trans = await dbContext.Database.BeginTransactionAsync())
                        {

                            var dirInfo = fileSystem.DirectoryInfo.New(recordPath);
                            var id = ItemBase.GetIdForPath(_mediaPath, dirInfo, Photo.IdPrefix);

                            // find the photo for this album's cover, if there is one
                            var coverPhoto = await dbContext.Photos.FirstOrDefaultAsync(p => p.ImportedShareId == record.Id);

                            await albumService.Edit(id, record.Title ?? dirInfo.Name, record.Description, coverPhoto?.Id);

                            dbContext.AlbumsDirNotFound.Remove(record);

                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception)
                {
                    // exception doesn't matter here as this record can be retried
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking albums not found metadata against the filesystem");
        }
    }


    /// <summary>
    /// Imports records exported from the `photo_comment` database table into OSPhoto
    /// </summary>
    /// <param name="csvFilename"></param>
    private async Task<bool> ImportPhotoComment(string csvFilename)
    {
        logger.LogInformation("Starting import of: {file}", csvFilename);

        var recordsImported = 0;
        var recordsNotFound = 0;

        try
        {
            // as the imported data doesn't have a timezone, default to current culture unless specified
            var importCultureInfo = !string.IsNullOrEmpty(_importTimezoneCulture)
                ? new CultureInfo(_importTimezoneCulture)
                : CultureInfo.CurrentCulture;

            using (var reader = new StreamReader(csvFilename))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                await csv.ReadAsync();
                csv.ReadHeader();

                while (await csv.ReadAsync())
                {
                    try
                    {
                        var record = csv.GetRecord<CsvPhotoCommentRecord>();

                        // trim the start of the path to match our config
                        var recordPath = record.Path;
                        if (recordPath.StartsWith(_synoSharePathPrefix))
                            recordPath = Path.Join(_mediaPath, recordPath.Substring(_synoSharePathPrefix.Length));

                        var createdDateUtc = DateTime
                            .ParseExact(record.Date, _importDateTimeFormat, importCultureInfo)
                            .ToUniversalTime();

                        if (File.Exists(recordPath))
                        {
                            var fsInfo = fileSystem.FileInfo.New(recordPath);
                            var prefix = fsInfo.IsImageFileType() ? Photo.IdPrefix : Video.IdPrefix;

                            if (await Import(ItemBase.GetIdForPath(_mediaPath, fsInfo, prefix), record, createdDateUtc))
                                recordsImported++;
                        }
                        else
                        {
                            logger.LogDebug(" > Photo file not found: {recordPath}", recordPath);
                            if (await ImportNotFound(record, createdDateUtc, csvFilename))
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
            logger.LogInformation(" > Saved information about {recordsFileNotFoundCount} comments that couldn't find the media file for.", recordsNotFound);

        return true;
    }

    private async Task<bool> Import(string id, CsvPhotoCommentRecord record, DateTime createdDate)
    {
        await dbContext.Comments.AddAsync(new DbComment(
            id,
            record.Comment,
            record.Name,
            record.Email
            ) { CreatedUtc = createdDate }
        );

        return true;
    }

    private async Task<bool> ImportNotFound(CsvPhotoCommentRecord record, DateTime createdDate, string csvFilename)
    {
        var cfnf = await dbContext
            .CommentsFileNotFound
            .FindAsync(record.Id);

        if (cfnf != null) return false;

        cfnf = new CommentFileNotFound(record, createdDate, csvFilename);

        await dbContext.CommentsFileNotFound.AddAsync(cfnf);

        return true;
    }

    /// <summary>
    /// Checks each record in the photo comments database against the file system to see if there's now a matching file
    /// (this could be because we'd imported photo comments, but were still copying over the photo files)
    /// </summary>
    private async Task CheckCommentsNotFound()
    {
        if (!await dbContext.CommentsFileNotFound.AnyAsync())
            return;

        try
        {
            for (int i = dbContext.CommentsFileNotFound.Count() - 1; i >= 0; i--)
            {
                try
                {
                    var record = await dbContext.CommentsFileNotFound.ElementAtAsync(i);

                    var recordPath = record.Path;
                    if (recordPath.StartsWith(_synoSharePathPrefix))
                        recordPath = Path.Join(_mediaPath, recordPath.Substring(_synoSharePathPrefix.Length));

                    if (File.Exists(recordPath))
                    {
                        using (var trans = await dbContext.Database.BeginTransactionAsync())
                        {
                            var fsInfo = fileSystem.FileInfo.New(recordPath);
                            var id = ItemBase.GetIdForPath(_mediaPath, fsInfo, Photo.IdPrefix);
                            await Import(id, new CsvPhotoCommentRecord(record), record.CreatedUtc);

                            dbContext.CommentsFileNotFound.Remove(record);

                            await dbContext.SaveChangesAsync();
                        }
                    }
                }
                catch (Exception)
                {
                    // exception doesn't matter here as this record can be retried
                }
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error checking photo comments not found against the filesystem");
        }
    }

}
