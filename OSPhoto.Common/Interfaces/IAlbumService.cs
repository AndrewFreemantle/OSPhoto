using Microsoft.Extensions.Logging;
using OSPhoto.Common.Models;

namespace OSPhoto.Common.Interfaces;

public interface IAlbumService
{
    AlbumResult Get(string path = "");
    Photo GetPhoto(string id);
    Task EditPhoto(string id, string title, string description);

    Task<Stream> GetThumbnail(string id);
    Task<bool> SetCoverPhoto(string albumId, string photoId);
    Task<bool> Edit(string albumId, string? title, string? description);
}
