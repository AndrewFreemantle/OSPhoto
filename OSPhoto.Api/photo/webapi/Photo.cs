namespace OSPhoto.Api.photo.webapi;


public class PhotoRequest : RequestBase
{
    public string Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public int? Rating { get; set; }    // todo: implement (1-5 stars)

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

public class PhotoResponseWithData(OSPhoto.Common.Models.ItemBase itemBase) : PhotoResponse
{
    public Common.Models.ItemBase[] Data { get; set; } = [itemBase];
}


public class Photo(IPhotoService photoService, IVideoService videoService) : Endpoint<PhotoRequest, PhotoResponse>
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

        IServiceBase service = req.Id.StartsWith(Common.Models.Photo.IdPrefix)
            ? photoService
            : videoService;

        switch (req.Method)
        {
            case RequestMethod.GetInfo:
                await SendAsync(new PhotoResponseWithData(service.GetInfo(req.Id)));
                break;
            case RequestMethod.Edit:
                await service.EditInfo(req.Id, req.Title, req.Description);
                await SendAsync(new PhotoResponse());
                break;
            case RequestMethod.Copy:    // app UI = "Move"
                foreach (var id in req.Id.Split(','))
                    await service.Move(id, req.DestinationAlbum, req.IsOverwrite);
                await SendAsync(new PhotoResponse());
                break;
            case RequestMethod.Delete:
                foreach (var id in req.Id.Split(','))
                    await service.Delete(id);
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
