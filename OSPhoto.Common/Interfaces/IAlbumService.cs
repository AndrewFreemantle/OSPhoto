using OSPhoto.Common.Models;

namespace OSPhoto.Common.Interfaces;

public interface IAlbumService
{
    string MediaPath { get; }
    AlbumResult Get(string path = "");
    Task<Stream> GetThumbnail(string id);
    Task<bool> SetCoverPhoto(string id, string photoId);
    Task<bool> Edit(string id, string? title, string? description, string? coverPhotoId = null);
    Task Create(string parentAlbumId, string albumName);
    Task<bool> Delete(string id);
}
