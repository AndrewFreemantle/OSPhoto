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

public class AlbumResponse
{
    private static readonly string RootAlbumId = $"album_{StringUtils.NewId()}";

    public bool Success { get; set; } = true;
    public AlbumResponseData Data { get; } = new();

    // TODO: Get this information from either the database or generate from the filesystem of media
    public AlbumResponse(bool root = true)
    {
        if (root)
            Data.Items = [new Item(
                RootAlbumId
                , "album"
                , new ItemInfo("Test Album", "Test Album", "Test Album")
                , new Additional() { Thumbnails = new Thumbnails() { Small = new ThumbnailInfo(200, 200, 1484649478)}}
                , "small"),
            new Item(
                $"photo_{StringUtils.NewId()}"
                , "photo"
                , new ItemInfo("Test Photo", "Test Photo.jpeg", "Test Photo")
                    {
                        CreateDate = "1970-01-01 00:00:00"
                        , TakenDate = "1970-01-01 00:00:00"
                        , Size = 2641377
                        , ResolutionX = 5420
                        , ResolutionY = 3614
                    }
                , new Additional() { Thumbnails = new Thumbnails() { Small = new ThumbnailInfo(300, 300, 1484649478)}}
                , "small")
            ];
        else
        {
            Data.Items = [new Item($"photo_{StringUtils.NewId()}"
                , "photo"
                , new ItemInfo("Photo 2", "Photo 2.jpeg", "Photo 2")
                {
                    CreateDate = "1970-01-01 00:00:00",
                    TakenDate = "1970-01-01 00:00:00",
                    Size = 2641377,
                    ResolutionX = 5420, ResolutionY = 3614
                }
                , new Additional() { Thumbnails = new Thumbnails() { Small = new ThumbnailInfo(300, 300, 1484649478) } }
                , "small")
            ];
        }
    }
}

public class Album() : Endpoint<AlbumRequest, AlbumResponse>
{
    public override void Configure()
    {
        Post("album.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(AlbumRequest req, CancellationToken ct)
    {
        Console.WriteLine($"Album called! (id: {req.Id})");
        await SendAsync(new AlbumResponse(req.Id == null));
    }
}
