using OSPhoto.Common.Interfaces;

namespace OSPhoto.Api.photo.webapi;

public class CoverRequest : RequestBase
{
    [BindFrom("id")]
    public string AlbumId { get; set; }
    [BindFrom("item_id")]
    public string PhotoId { get; set; }
}

public class CoverResponse(bool success)
{
    public bool Success => success;
}

public class Cover(IAlbumService service) : Endpoint<CoverRequest, CoverResponse>
{
    public override void Configure()
    {
        Post("cover.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(CoverRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Cover (albumId: {albumId}, photoId: {photoId})", req.AlbumId, req.PhotoId);

        switch (req.Method)
        {
            case RequestMethod.Set:
                try
                {
                    await SendAsync(new CoverResponse(await service.SetCoverPhoto(req.AlbumId, req.PhotoId)));
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception setting cover photo for album id: {albumId}", req.AlbumId);
                    await SendAsync(new CoverResponse(false));
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
