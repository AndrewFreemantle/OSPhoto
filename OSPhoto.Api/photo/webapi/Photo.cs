using OSPhoto.Api.Extensions;
using OSPhoto.Common.Interfaces;

namespace OSPhoto.Api.photo.webapi;


public class PhotoRequest : RequestBase
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
}

public class PhotoResponse
{
    public bool Success => true;
}

public class PhotoResponseWithData(OSPhoto.Common.Models.Photo photo) : PhotoResponse
{
    public Common.Models.Photo[] Data { get; set; } = new[] { photo };
}


public class Photo(IAlbumService albumService) : Endpoint<PhotoRequest, PhotoResponse>
{
    public override void Configure()
    {
        Post("photo.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(PhotoRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Photo (method: {method})", req.Method);

        switch (req.Method)
        {
            case RequestMethod.GetInfo:
                var photo = albumService.GetPhoto(req.Id);
                await SendAsync(new PhotoResponseWithData(photo));
                break;
            case RequestMethod.Edit:
                await albumService.EditPhoto(req.Id, req.Title, req.Description);
                await SendAsync(new PhotoResponse());
                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}" +
                                "\n > body: {body}",
                    req.Method,
                    await HttpContext.Request.Body.ReadAsStringAsync());
                await SendNoContentAsync();
                break;
        }
    }
}
