using FastEndpoints.Swagger;
using Hangfire;
using HeyRed.ImageSharp.Heif;
using HeyRed.ImageSharp.Heif.Formats.Heif;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NSwag;
using OSPhoto.Api.Authentication;
using OSPhoto.Api.Processors;
using OSPhoto.Common.Database;
using OSPhoto.Common.Services;
using OSPhoto.Common.Configuration;
using SixLabors.ImageSharp;

var builder = WebApplication.CreateBuilder(args);

// Configure configuration sources
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>();

// Configure strongly-typed settings using Options pattern
builder.Services.Configure<AppSettings>(options => new AppSettings());

// Add configuration validation
builder.Services.AddOptions<AppSettings>()
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddFastEndpoints()
    .AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
    {
        var settings = serviceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
        options.UseSqlite(settings.DatabaseConnectionString);
    })
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
    .AddScoped<IConfigurationService, ConfigurationService>()
    .AddScoped<IImportService, ImportService>()
    .AddScoped<IUserService, UserService>()
    .AddScoped<IAlbumService, AlbumService>()
    .AddScoped<IPhotoService, PhotoService>()
    .AddScoped<IVideoService, VideoService>()
    .AddScoped<ICommentService, CommentService>()
    .AddScoped<IStatsService, StatsService>()
    .AddSingleton<IFileSystem, FileSystem>();

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
    // Ensure the database is created
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var settings = scope.ServiceProvider.GetRequiredService<IOptions<AppSettings>>().Value;
        dbContext.CreateDatabase(settings.DatabasePath);
        dbContext.SeedUsers(settings.Users);

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
