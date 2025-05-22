using System.ComponentModel.DataAnnotations;

namespace OSPhoto.Common.Configuration;

public class AppSettings
{
    public AppSettings()
    {
        // Required paths
        MediaPath = Environment.GetEnvironmentVariable("MEDIA_PATH") ?? "/Media";
        AppDataPath = Environment.GetEnvironmentVariable("APPDATA_PATH") ?? "/AppData";
        DatabasePath = Environment.GetEnvironmentVariable("DATABASE_PATH") ?? Path.Join(AppDataPath, "database", "osphoto.db");
        ImportPath = Environment.GetEnvironmentVariable("IMPORT_PATH") ?? Path.Join(AppDataPath, "import");

        // Database
        DatabaseConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? $"Data Source={DatabasePath}";

        // Additional settings
        Users = Environment.GetEnvironmentVariable("USERS");
        AllowComments = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ALLOW_COMMENTS"));
        SynoSharePathPrefix = Environment.GetEnvironmentVariable("SYNO_SHARE_PATH_PREFIX") ?? "/volume1/photo/";
        ThumbnailWidthInPixels = int.Parse(Environment.GetEnvironmentVariable("THUMB_WIDTH_PX") ?? "500");
        ImportTimezoneCulture = Environment.GetEnvironmentVariable("IMPORT_TIMEZONE_CULTURE");
    }

    [Required]
    public string MediaPath { get; }

    [Required]
    public string AppDataPath { get; }

    [Required]
    public string DatabasePath { get; }

    [Required]
    public string ImportPath { get; }

    [Required]
    public string DatabaseConnectionString { get; }

    public string? Users { get; }

    public bool AllowComments { get; }

    public string SynoSharePathPrefix { get; }

    public int ThumbnailWidthInPixels { get; }

    public string? ImportTimezoneCulture { get; }
}
