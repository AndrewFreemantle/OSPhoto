using OSPhoto.Common.Models;

namespace OSPhoto.Common.Interfaces;

public interface IVideoService : IServiceBase
{
    Task<Stream> GetThumbnail(string id, string? size);
}
