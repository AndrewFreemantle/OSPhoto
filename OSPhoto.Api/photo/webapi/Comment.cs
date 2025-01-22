using DbComment = OSPhoto.Common.Database.Models.Comment;

namespace OSPhoto.Api.photo.webapi;

public class CommentRequest : RequestBase
{
    public string Id { get; set; } = string.Empty;

    // create properties
    public string Comment { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; } = null;
}

public class CommentResponse
{
    public bool Success { get; set; } = true;
}

public class CommentListResponse(List<DbComment> dbComments) : CommentResponse
{
    public CommentListResponseData Data { get; set; } = new (dbComments);
}

public class CommentListResponseData(List<DbComment> dbComments)
{
    public List<SingleCommentResponse> Comments { get; set; } = dbComments.Select(c => new SingleCommentResponse(c)).ToList();
}

public class SingleCommentResponse(DbComment dbComment)
{
    public string Id { get; set; } = dbComment.Id.ToString();
    public string Name { get; set; } = dbComment.Name;
    public string? Email { get; set; } = dbComment.Email;
    public string Comment { get; set; } = dbComment.Text;
    public string Date { get; set; } = dbComment.CreatedUtc.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss");
}

public class CommentCreateResponse : CommentResponse
{
    public CommentCreateResponse(int result)
    {
        Success = result > 0;
        Data = new(result);
    }

    public CommentCreateResponseData Data { get; set; }
}

public class CommentCreateResponseData(int result)
{
    public int Id { get; set; } = result;
}


public class Comment(ICommentService service, IUserService userService) : Endpoint<CommentRequest, CommentResponse>
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
                await SendAsync(new CommentListResponse(await service.Get(req.Id)));
                break;
            case RequestMethod.Create:
                var result = await service.Create(req.Id, req.Comment, req.Name, req.Email);
                await SendAsync(new CommentCreateResponse(result));
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
