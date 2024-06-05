namespace OSPhoto.Common.Services.Models;

public class StatsResponse(int albumsCount, int albumsNotFoundCount, int photosCount, int photosNotFoundCount)
{
    public string ReadMe { get; } =
        "Statistics from the OSPhoto database - these counts are for database metadata, OSPhoto doesn't need a record for every photo or album so this is NOT the true size of your entire collection";

    public int Albums { get; set; } = albumsCount;
    public int AlbumsDirNotFound { get; set; } = albumsNotFoundCount;

    public int Photos { get; set; } = photosCount;
    public int PhotosFileNotFound { get; set; } = photosNotFoundCount;
}
