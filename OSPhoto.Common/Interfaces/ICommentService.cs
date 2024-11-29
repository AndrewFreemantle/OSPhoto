using OSPhoto.Common.Database.Models;
using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Interfaces;

public interface ICommentService
{
    Task<List<Comment>> Get(string mediaId);
    Task<int> Create(string mediaId, string text, string name, string? email);

    Task<MoveResult> Move(string oldMediaId, string newMediaId);
    Task Delete(string mediaId);
}
