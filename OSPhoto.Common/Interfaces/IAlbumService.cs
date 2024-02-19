using Microsoft.Extensions.Logging;

namespace OSPhoto.Common.Interfaces;

public interface IAlbumService
{
    AlbumResult Get(string path = "");
    Image GetImage(string path);

    void SetLogger(ILogger logger);
    Stream GetThumbnail(string id);
}
