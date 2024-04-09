namespace OSPhoto.Api;

/// <summary>
/// Catch-all / unknown / unimplemented endpoint request handler.
/// Logs all given request information to aid the development of missing API features.
/// </summary>
public class CatchAllEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Verbs(Http.GET, Http.PUT, Http.HEAD, Http.POST, Http.PATCH, Http.DELETE);
        Routes("/{**path}");
        RoutePrefixOverride(string.Empty);
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var body = await HttpContext.Request.Body.ReadAsStringAsync();
        var headers = string.Join(' ', HttpContext.Request.Headers.Select(h => $"\n\t{h.Key} = {h.Value.ToString()}"));
        var cookies = string.Join(' ', HttpContext.Request.Cookies.Select(c => $"\n\t{c.Key} = {c.Value}"));

        Logger.LogError("Catch-all Request: {method} {path}" +
            "\n > query: {query}" +
            "\n > body: {body}" +
            "\n > headers: {headers}" +
            "\n > cookies: {cookies}"
           , HttpContext.Request.Method
           , HttpContext.Request.Path
           , HttpContext.Request.QueryString
           , body
           , headers
           , cookies);

        await SendNotFoundAsync();

        /*
         Request: POST /photo/webapi/query.php
          > body: api=SYNO.API.Info&method=query&version=1&query=all
         Request: POST /photo/webapi/auth.php
          > body: api=SYNO.PhotoStation.Auth&method=login&version=1&username=Andrew&password={PASSWORD HERE}
        Request: POST /photo/webapi/info.php
          > body: api=SYNO.PhotoStation.Info&method=getinfo&version=1&PHPSESSID={GUID WITHOUT DASHES}
        Request: POST /photo/webapi/album.php
           > body: api=SYNO.PhotoStation.Album&method=list&version=1&offset=0&sort_by=preference&sort_direction=asc&limit=108&additional=album_permission%2Cvideo_codec%2Cvideo_quality%2Cthumb_size%2Cphoto_exif&type=album%2Cphoto%2Cvideo&PHPSESSID={GUID WITHOUT DASHES}
        Request: GET /photo/webapi/thumb.php
           > query string: ?api=SYNO.PhotoStation.Thumb&method=get&version=1&id=album_{GUID WITHOUT DASHES}&size=small&PHPSESSID={GUID WITHOUT DASHES}&mtime=1484649478&sig={SOME REALLY-REALLY LONG STRING THAT LOOKS LIKE 4x-5x GUID CONCATENATIONS}
           < returns the binary file photo thumbnail

        // TODO: implement category...
        Request: POST /photo/webapi/category.php
           > body: api=SYNO.PhotoStation.Category&method=list&version=1&limit=2147483647&PHPSESSID={GUID WITHOUT DASHES}&offset=0
           < {"success":true,"data":{"total":0,"offset":0,"categories":[]}}

        // Sub-Album Selection
        Request: POST /photo/webapi/album.php
           > body: api=SYNO.PhotoStation.Album&method=list&version=1&offset=0&sort_by=preference&id=album_{GUID WITHOUT DASHES}&sort_direction=asc&limit=108&additional=album_permission%2Cvideo_codec%2Cvideo_quality%2Cthumb_size%2Cphoto_exif&type=album%2Cphoto%2Cvideo&PHPSESSID={GUID WITHOUT DASHES}

        // Photo
        Request: POST /photo/webapi/photo.php
            > body: api=SYNO.PhotoStation.Photo&method=getinfo&version=1&id={PHOTO ID}&additional=photo_exif&PHPSESSID={SESSION ID}

        // TODO: implement comment...
        Request: POST /photo/webapi/comment.php
           > body: api=SYNO.PhotoStation.Comment&method=list&version=1&id={PHOTO ID}&PHPSESSID={SESSION ID}
           < {"success":true,"data":{"comments":[]}}

        // TODO: implement cover... (set a cover photo for an album)
        Request: POST /photo/webapi/cover.php
           > body: api=SYNO.PhotoStation.Cover&method=set&version=1&id={ALBUM ID}&item_id={PHOTO_ID}&PHPSESSID={SESSION ID}


        // TODO: implement tag...
        Request: POST /photo/webapi/tag.php
           > body: api=SYNO.PhotoStation.Tag&method=list&version=1&PHPSESSID={SESSION ID}&additional=info&offset=0&type=desc&limit=-1
           < {"success":true,"data":{"total":1,"offset":1,"tags":[{"id":"tag_1","type":"tag","tag_type":"desc","name":"SOME TAG NAME (e.g. vibrant)","additional":{"info":{"name":"SOME TAG NAME (e.g. vibrant)"}}}]}}

        // TODO: implement photo_tag...
        Request: POST /photo/webapi/photo_tag.php
           > body: api=SYNO.PhotoStation.PhotoTag&method=list&version=1&PHPSESSID={SESSION_ID}&id={PHOTO ID}&additional=info&type=people%2Cdesc%2Cgeo
           < {"success":true,"data":{"tags":[]}}
        */
    }
}
