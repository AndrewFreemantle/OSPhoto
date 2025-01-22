using OSPhoto.Common.Extensions;
using OSPhoto.Common.Models;
using AlbumModel = OSPhoto.Common.Models.Album;
using PhotoModel = OSPhoto.Common.Models.Photo;
using VideoModel = OSPhoto.Common.Models.Video;

namespace OSPhoto.Api.photo.webapi;


public class DownloadRequest : RequestBase
{
    public string Id { get; set; }

    [BindFrom("quality_id")]
    public string QualityId { get; set; } = string.Empty;
}


public class Download(IAlbumService albumService, IFileSystem fileSystem) : Endpoint<DownloadRequest>
{
    public override void Configure()
    {
        Get("download.php");
    }

    public override async Task HandleAsync(DownloadRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Thumb (id: {id}, method: {method})", req.Id, req.Method);

        switch (req.Method)
        {
            case RequestMethod.GetVideo:
                try
                {
                    var videoPath = Path.Combine(albumService.MediaPath, ItemBase.GetPathFromId(req.Id));
                    var fi = fileSystem.FileInfo.New(videoPath);

                    // Set the Cache-Control header
                    HttpContext.Response.Headers["Cache-Control"] = $"max-age={3600 * 24 * 7}"; // one week

                    // Stream the file content to the response
                    using (var stream = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read))
                    {
                        await SendStreamAsync(stream, fi.Name.Replace('"', '_'), fi.Length, fi.ContentType(), fi.LastModifiedOffsetUtc(), true);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception downloading method: {method}, id: {id}", req.Method, req.Id);
                    await SendNotFoundAsync();
                }

                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}" +
                                "\n > query: {query}",
                    req.Method,
                    HttpContext.Request.QueryString);
                await SendNoContentAsync();
                break;
        }
    }
}
