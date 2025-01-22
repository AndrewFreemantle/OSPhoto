using AlbumModel = OSPhoto.Common.Models.Album;
using PhotoModel = OSPhoto.Common.Models.Photo;
using VideoModel = OSPhoto.Common.Models.Video;

namespace OSPhoto.Api.photo.webapi;

public class ThumbRequest : RequestBase
{
    public string Id { get; set; }
    public string Size { get; set; }
    [BindFrom("mtime")]
    public string MTime { get; set; }
    [BindFrom("sig")]
    public string Signature { get; set; }
}

public class Thumb(IAlbumService albumService, IPhotoService photoService, IVideoService videoService) : Endpoint<ThumbRequest>
{
    public override void Configure()
    {
        Get("thumb.php");
    }

    public override async Task HandleAsync(ThumbRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Thumb (id: {id}, size: {size}, sig: {sig})", req.Id, req.Size, req.Signature);

        switch (req.Method)
        {
            case RequestMethod.Get:
                try
                {
                    if (req.Id.StartsWith(AlbumModel.IdPrefix))
                        await SendStreamAsync(await albumService.GetThumbnail(req.Id));
                    else if (req.Id.StartsWith(PhotoModel.IdPrefix))
                        await SendStreamAsync(await photoService.GetThumbnail(req.Id));
                    else if (req.Id.StartsWith(VideoModel.IdPrefix))
                        await SendStreamAsync(await videoService.GetThumbnail(req.Id, req.Size));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception returning thumbnail for id: {id}", req.Id);
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
