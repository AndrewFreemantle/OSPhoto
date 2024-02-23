using Microsoft.Extensions.Logging;
using OSPhoto.Common.Models;

namespace OSPhoto.Common.Interfaces;

public interface IAlbumService
{
    AlbumResult Get(string path = "");
    Photo GetImage(string path);

    Stream GetThumbnail(string id);
}
