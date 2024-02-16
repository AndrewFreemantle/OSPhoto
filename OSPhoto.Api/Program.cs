using System.Text;
using Microsoft.AspNetCore.Authentication;
using FastEndpoints.Swagger;
using NSwag;
using OSPhoto.Api.Authentication;
using OSPhoto.Api.Processors;
using OSPhoto.Common;
using OSPhoto.Common.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddFastEndpoints()
    .AddAuthorization()
    .AddAuthentication(SessionAuth.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, SessionAuth>(SessionAuth.SchemeName, null);

builder.Services.SwaggerDocument(o =>
{
    o.EnableJWTBearerAuth = false;
    o.DocumentSettings = s =>
    {
        s.AddAuth(SessionAuth.SchemeName, new()
        {
            Name = SessionAuth.SessionPropertyName,
            In = OpenApiSecurityApiKeyLocation.Query,
            Type = OpenApiSecuritySchemeType.ApiKey,
        });
    };
});

// Register implementations
var contentRootPath = Environment.GetEnvironmentVariable("MEDIA_ROOT") ?? Path.GetFullPath("../Media/");
builder.Services.AddSingleton<IAlbumService>(_ => new AlbumService(contentRootPath));

var app = builder.Build();

// TODO: is there a way to do this at registration (line 38 above?)
var albumService = app.Services.GetService(typeof(IAlbumService)) as IAlbumService;
albumService.SetLogger(app.Logger);


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "photo/webapi";
    c.Endpoints.Configurator = ep =>
    {
        ep.PreProcessor<AdditionalResponseHeadersPreProcessor>(Order.Before);
    };
});

// TODO: move to a catch-all class: https://fast-endpoints.com/docs/misc-conveniences#multiple-verbs-routes
app.MapMethods("/{**path}", new[] { "GET", "POST", "PUT", "DELETE", "PATCH", "OPTIONS" }, async (ctx) =>
    {
        string body = null;
        if (ctx.Request.Body != null)
        {
            using (var reader = new StreamReader(ctx.Request.Body, Encoding.UTF8))
            {
                body = await reader.ReadToEndAsync();
            }
        }

        var headers = string.Join(' ', ctx.Request.Headers.Select(h => $"\n\t{h.Key} = {h.Value.ToString()}"));
        var cookies = string.Join(' ', ctx.Request.Cookies.Select(c => $"\n\t{c.Key} = {c.Value}"));

        Console.WriteLine($"Catch-all Request: {ctx.Request.Method} {ctx.Request.Path}" +
            $"{(ctx.Request.QueryString.HasValue ? "\n > query string: " + ctx.Request.QueryString : "")}" +
            $"\n > body: {body}" +
            $"\n > headers: {headers}" +
            $"\n > cookies: {cookies}");

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

        Sub-Album Selection
        Request: POST /photo/webapi/album.php
           > body: api=SYNO.PhotoStation.Album&method=list&version=1&offset=0&sort_by=preference&id=album_{GUID WITHOUT DASHES}&sort_direction=asc&limit=108&additional=album_permission%2Cvideo_codec%2Cvideo_quality%2Cthumb_size%2Cphoto_exif&type=album%2Cphoto%2Cvideo&PHPSESSID={GUID WITHOUT DASHES}
        */

    })
    .WithName("CatchAll")
    .WithOpenApi();

app.Run();
