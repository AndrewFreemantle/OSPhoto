using System.Text.Json;

namespace OSPhoto.Api.photo.webapi;

public class PhotoTagRequest : RequestBase
{
    public string Additional { get; set; }
    public string Type { get; set; }
}

public class PhotoTagResponse
{
    private string _data = @"{""tags"":[]}";

    public bool Success => true;
    public JsonDocument Data => JsonDocument.Parse(_data);
}

public class PhotoTag : Endpoint<PhotoTagRequest, PhotoTagResponse>
{
    public override void Configure()
    {
        Post("photo_tag.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(PhotoTagRequest req, CancellationToken ct)
    {
        Logger.LogInformation("PhotoTag (method: {method}, additional: {additional}, type: {type})",
            req.Method, req.Additional, req.Type);

        switch (req.Method)
        {
            case RequestMethod.List:
                await SendAsync(new PhotoTagResponse());
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
