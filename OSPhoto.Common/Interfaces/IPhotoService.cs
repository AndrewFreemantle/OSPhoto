using Microsoft.AspNetCore.Http;
using OSPhoto.Common.Models;

namespace OSPhoto.Common.Interfaces;

public interface IPhotoService
{
    Photo GetInfo(string id);
    Task EditInfo(string id, string title, string description);
    Task<bool> Upload(IFormFile file, string destinationAlbum, string fileName, string? title, string? description);
    Task Delete(string id);
    Task<bool> Move(string id, string destinationAlbumId, bool isOverwrite);
}
