using System.Text.Json;

namespace OSPhoto.Api.photo.webapi;

public class CommentRequest : RequestBase
{
    public string Id { get; set; }
}

public class CommentResponse
{
    private static string _data = @"{""comments"":[]}";

    public bool Success { get; set; } = true;
    public JsonDocument Data => JsonDocument.Parse(_data);
}


public class Comment : Endpoint<CommentRequest, CommentResponse>
{
    public override void Configure()
    {
        Post("comment.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(CommentRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Comment (method: {method}, id: {id})", req.Method, req.Id);

        switch (req.Method)
        {
            case RequestMethod.List:
                await SendAsync(new CommentResponse());
                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}"
                    , req.Method);
                await SendNoContentAsync();
                break;
        }
    }
}
