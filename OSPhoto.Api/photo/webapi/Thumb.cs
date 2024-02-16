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

public class Thumb : Endpoint<ThumbRequest>
{
    public override void Configure()
    {
        Get("thumb.php");
    }

    public override async Task HandleAsync(ThumbRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Thumb (id: {id})", req.Id);

        switch (req.Method)
        {
            case RequestMethod.Get:
                try
                {
                    var filePath = File.Exists("../Media/pexels-andre-furtado-1264210-thumb.jpg") ? "../Media/pexels-andre-furtado-1264210-thumb.jpg" /* command line */ : "../../../Media/pexels-andre-furtado-1264210-thumb.jpg"; /* debugger */
                    var fileInfo = new FileInfo(filePath);

                    HttpContext.MarkResponseStart();
                    HttpContext.Response.StatusCode = 200;
                    HttpContext.Response.ContentType = "image/jpeg";
                    // await File.ReadAllBytesAsync(filePath);

                    await SendFileAsync(fileInfo);
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Exception returning thumbnail for id: {id}", req.Id);
                    await SendNotFoundAsync();
                }

                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}"
                    , req.Method);
                await SendNoContentAsync();
                break;
        }
    }
}
