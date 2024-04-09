using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Models;

namespace OSPhoto.Api.photo.webapi;

public class AlbumRequest  : RequestBase
{
    public int Offset { get; set; } = 0;
    [JsonPropertyName("sort_by")]
    public string SortBy { get; set; }
    [JsonPropertyName("sort_direction")]
    public string SortDirection { get; set; }
    public string Id { get; set; }
    public int Limit { get; set; } = 108;
    public string Additional { get; set; }
    public string Type { get; set; }

    // edit properties
    public string? Title { get; set; }
    public string? Description { get; set; }
}

public class AlbumResponse(IAlbumService albumService, string id)
{
    public bool Success { get; set; } = true;
    public AlbumResult Data => albumService.Get(id);
}

public class Album(IAlbumService service) : Endpoint<AlbumRequest, AlbumResponse>
{
    public override void Configure()
    {
        Post("album.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(AlbumRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Album (method: {method}, id: {id})"
            , req.Method
            , req.Id);

        switch (req.Method)
        {
            case RequestMethod.List:
                await SendAsync(new AlbumResponse(service, req.Id));
                break;
            case RequestMethod.Edit:
                await service.Edit(req.Id, req.Title, req.Description);
                await SendAsync(new AlbumResponse(service, req.Id));
                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}" +
                                "\n > body: {body}",
                    req.Method,
                    await HttpContext.Request.Body.ReadAsStringAsync());
                await SendNotFoundAsync();
                break;
        }
    }
}
