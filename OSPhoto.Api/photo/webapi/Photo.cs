using OSPhoto.Common.Interfaces;

namespace OSPhoto.Api.photo.webapi;


public class PhotoRequest : RequestBase
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }

    [BindFrom("sharepath")]
    public string? DestinationAlbum { get; set; }
    public string? Mode { get; set; }  // "move"
    public string? Duplicate { get; set; }  // "ignore" (skip)
    public bool IsOverwrite => !string.IsNullOrEmpty(Duplicate) && Duplicate != "ignore";
}

public class PhotoResponse(bool success = true)
{
    public bool Success => success;
}

public class PhotoResponseWithData(OSPhoto.Common.Models.Photo photo) : PhotoResponse
{
    public Common.Models.Photo[] Data { get; set; } = new[] { photo };
}


public class Photo(IAlbumService albumService, IPhotoService photoService) : Endpoint<PhotoRequest, PhotoResponse>
{
    public override void Configure()
    {
        Post("photo.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(PhotoRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Photo (method: {method}, id: {id})"
            , req.Method
            , req.Id);

        switch (req.Method)
        {
            case RequestMethod.GetInfo:
                var photo = photoService.GetInfo(req.Id);
                await SendAsync(new PhotoResponseWithData(photo));
                break;
            case RequestMethod.Edit:
                await photoService.EditInfo(req.Id, req.Title, req.Description);
                await SendAsync(new PhotoResponse());
                break;
            case RequestMethod.Copy:    // app UI = "Move"
                foreach (var id in req.Id.Split(','))
                    await photoService.Move(id, req.DestinationAlbum, req.IsOverwrite);
                await SendAsync(new PhotoResponse());
                break;
            case RequestMethod.Delete:
                foreach (var id in req.Id.Split(','))
                    await photoService.Delete(id);
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
