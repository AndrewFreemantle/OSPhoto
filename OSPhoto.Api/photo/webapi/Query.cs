namespace OSPhoto.Api.photo.webapi;

using System.Text.Json;

public class QueryRequest : RequestBase
{
}

public record QueryResponse
{
    private static string data =
        @"{""SYNO.PhotoStation.Auth"":{""path"":""auth.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Info"":{""path"":""info.php"",""minVersion"":1,""maxVersion"":2},""SYNO.PhotoStation.Album"":{""path"":""album.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Permission"":{""path"":""permission.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Photo"":{""path"":""photo.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Thumb"":{""path"":""thumb.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Cover"":{""path"":""cover.php"",""minVersion"":1,""maxVersion"":2},""SYNO.PhotoStation.SmartAlbum"":{""path"":""smart_album.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.File"":{""path"":""file.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Download"":{""path"":""download.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Category"":{""path"":""category.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.About"":{""path"":""about.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Tag"":{""path"":""tag.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.PhotoTag"":{""path"":""photo_tag.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Comment"":{""path"":""comment.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Timeline"":{""path"":""timeline.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Group"":{""path"":""group.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Rotate"":{""path"":""rotate.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.SlideshowMusic"":{""path"":""slideshow_music.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.DsmShare"":{""path"":""dsm_share.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.SharedAlbum"":{""path"":""shared_album.php"",""minVersion"":1,""maxVersion"":2},""SYNO.PhotoStation.PhotoLog"":{""path"":""log.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Path"":{""path"":""path.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Watermark"":{""path"":""watermark.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Public"":{""path"":""public.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.Migration"":{""path"":""migration.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.ACL"":{""path"":""acl.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.AdvancedShare"":{""path"":""advanced_share.php"",""minVersion"":1,""maxVersion"":1},""SYNO.PhotoStation.AppPrivilege"":{""path"":""app_privilege.php"",""minVersion"":1,""maxVersion"":1},""SYNO.API.Info"":{""path"":""query.php"",""minVersion"":1,""maxVersion"":1}}";

    public bool Success { get; set; } = true;
    public JsonDocument Data { get; set; } = JsonDocument.Parse(data);
}

public class Query : Endpoint<QueryRequest, QueryResponse>
{
    public override void Configure()
    {
        Post("query.php");
        AllowFormData(urlEncoded: true);
        AllowAnonymous();
    }

    public override async Task HandleAsync(QueryRequest req, CancellationToken ct)
    {
        // api=SYNO.API.Info&method=query&version=1&query=all
        Logger.LogInformation("Query (method: {method})", req.Method);

        switch (req.Method)
        {
            case RequestMethod.Query:
                await SendAsync(new QueryResponse());
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
