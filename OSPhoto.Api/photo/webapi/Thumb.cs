using Microsoft.AspNetCore.Mvc;
using OSPhoto.Common.Interfaces;

namespace OSPhoto.Api.photo.webapi;

public class ThumbRequest : RequestBase
{
    public string Id { get; set; }
    public string Size;
    [BindFrom("mtime")]
    public string MTime { get; set; }
    [BindFrom("sig")]
    public string Signature { get; set; }
}

public class Thumb(IAlbumService service) : Endpoint<ThumbRequest>
{
    public override void Configure()
    {
        Get("thumb.php");
    }

    public override async Task HandleAsync(ThumbRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Thumb (id: {id}, sig: {sig})", req.Id, req.Signature);

        switch (req.Method)
        {
            case RequestMethod.Get:
                try
                {
                    await SendStreamAsync(await service.GetThumbnail(req.Id));
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
