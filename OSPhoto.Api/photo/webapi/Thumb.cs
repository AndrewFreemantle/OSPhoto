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
        Console.WriteLine($"Thumb CALLED! (id: {req.Id})");

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
        catch (Exception e)
        {
            Console.WriteLine(e);
            await SendNotFoundAsync();
        }
    }
}
