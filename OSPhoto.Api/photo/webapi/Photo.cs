using OSPhoto.Common.Interfaces;

namespace OSPhoto.Api.photo.webapi;


public class PhotoRequest : RequestBase
{
    public string Id { get; set; }
}

public class PhotoResponse(OSPhoto.Common.Models.Photo photo)
{
    public bool Success => true;
    public OSPhoto.Common.Models.Photo[] Data { get; set; } = new[] { photo };
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
                await SendAsync(new PhotoResponse(photo));
                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}"
                    , req.Method);
                await SendNoContentAsync();
                break;
        }
    }
}
