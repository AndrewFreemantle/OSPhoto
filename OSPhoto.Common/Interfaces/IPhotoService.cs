using Microsoft.AspNetCore.Http;
using OSPhoto.Common.Models;

namespace OSPhoto.Common.Interfaces;

public interface IPhotoService : IServiceBase
{
    Task<Stream> GetThumbnail(string id);
    Task<bool> Upload(IFormFile file, string destinationAlbum, string fileName, string? title, string? description);
}
