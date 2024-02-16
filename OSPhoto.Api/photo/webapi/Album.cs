using OSPhoto.Common;
using OSPhoto.Common.Interfaces;

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
}

public class AlbumResponse(IAlbumService albumService, string id)
{
    public bool Success { get; set; } = true;
    public AlbumResult Data => albumService.Get(id);
}

public class Album(IAlbumService albumService) : Endpoint<AlbumRequest, AlbumResponse>
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
                await SendAsync(new AlbumResponse(albumService, req.Id));
                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}"
                    , req.Method);
                await SendNotFoundAsync();
                break;
        }
    }
}
