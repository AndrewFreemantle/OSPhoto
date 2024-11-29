using OSPhoto.Common.Models;
using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Interfaces;

/// <summary>
/// A common interface for media services
/// </summary>
public interface IServiceBase
{
    ItemBase GetInfo(string id);
    Task EditInfo(string id, string title, string description, int? importedShareId = null);
    Task<MoveResult> Move(string id, string destinationAlbumId, bool isOverwrite);
    Task Delete(string id);
}
