using FastEndpoints.Swagger;
using Hangfire;
using HeyRed.ImageSharp.Heif;
using HeyRed.ImageSharp.Heif.Formats.Heif;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using NSwag;
using OSPhoto.Api.Authentication;
using OSPhoto.Api.Processors;
using OSPhoto.Common.Database;
using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Services;
using SixLabors.ImageSharp;

var builder = WebApplication.CreateBuilder(args);

const string MEDIA_PATH = "MEDIA_PATH";             // full path to the media (photos & videos)
const string APPDATA_PATH = "APPDATA_PATH";         // full path for OSPhoto's application data
const string DATABASE_PATH = "DATABASE_PATH";       // full path for OSPhoto's database
const string IMPORT_PATH = "IMPORT_PATH";           // full path for photo description import CSV files

const string DATABASE_SUB_PATH = "database";
const string DATABASE_FILENAME = "osphoto.db";

const string IMPORT_SUB_PATH = "import";

string mediaPath = Environment.GetEnvironmentVariable(MEDIA_PATH) ?? "/Media";
string appDataPath = Environment.GetEnvironmentVariable(APPDATA_PATH) ?? "/AppData";
string databasePath = Environment.GetEnvironmentVariable(DATABASE_PATH)
                      ?? Path.Join(appDataPath, DATABASE_SUB_PATH, DATABASE_FILENAME); // default: /AppData/database/osphoto.db
string importPath = Environment.GetEnvironmentVariable(IMPORT_PATH)
                    ?? Path.Join(appDataPath, IMPORT_SUB_PATH);

Environment.SetEnvironmentVariable(MEDIA_PATH, mediaPath);
Environment.SetEnvironmentVariable(APPDATA_PATH, appDataPath);
Environment.SetEnvironmentVariable(DATABASE_PATH, databasePath);
Environment.SetEnvironmentVariable(IMPORT_PATH, importPath);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddFastEndpoints()
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlite($"Data Source={databasePath};Version=3")
    )
    .AddHangfire(configuration => configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UseInMemoryStorage())
    .AddHangfireServer()
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
builder.Services
    .AddScoped<IImportService, ImportService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IAlbumService, AlbumService>()
    .AddScoped<IPhotoService, PhotoService>()
    .AddScoped<IVideoService, VideoService>()
    .AddScoped<IStatsService, StatsService>();

// Configure Kestrel
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Limits.MaxRequestBodySize = null;     // disable the file upload limit
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseAuthorization();

app.UseFastEndpoints(c =>
{
    c.Endpoints.RoutePrefix = "photo/webapi";
    c.Endpoints.Configurator = ep =>
    {
        ep.PreProcessor<AdditionalResponseHeadersPreProcessor>(Order.Before);
    };
});

app.Lifetime.ApplicationStarted.Register(async () =>
{
    // Ensure the the database is created
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        dbContext.CreateDatabase(databasePath);
        dbContext.SeedUsers(Environment.GetEnvironmentVariable("USERS"));

        // Check for data to import...
        var backgroundJobs = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
        var importService = scope.ServiceProvider.GetRequiredService<IImportService>();
        backgroundJobs.Enqueue(() => importService.RunAsync());
    }

    // Register HEIF image support globally
    Configuration.Default.ImageFormatsManager.AddImageFormat(HeifFormat.Instance);
    Configuration.Default.ImageFormatsManager.AddImageFormatDetector(new HeifImageFormatDetector());
    Configuration.Default.ImageFormatsManager.SetDecoder(HeifFormat.Instance, HeifDecoder.Instance);
    Configuration.Default.ImageFormatsManager.SetEncoder(HeifFormat.Instance, new HeifEncoder());
});

app.Run();
