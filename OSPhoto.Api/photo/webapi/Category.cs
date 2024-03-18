using System.Text.Json;

namespace OSPhoto.Api.photo.webapi;

public class CategoryRequest : RequestBase
{
    public int Limit { get; set; }
}

public class CategoryResponse
{
    private static string _data = @"{""total"":0,""offset"":0,""categories"":[]}";

    public bool Success { get; set; } = true;
    public JsonDocument Data => JsonDocument.Parse(_data);
}

public class Category : Endpoint<CategoryRequest, CategoryResponse>
{
    public override void Configure()
    {
        Post("category.php");
        AllowFormData(urlEncoded: true);
    }

    public override async Task HandleAsync(CategoryRequest req, CancellationToken ct)
    {
        Logger.LogInformation("Category (method: {method}, limit: {limit})", req.Method, req.Limit);

        switch (req.Method)
        {
            case RequestMethod.List:
                await SendAsync(new CategoryResponse());
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
