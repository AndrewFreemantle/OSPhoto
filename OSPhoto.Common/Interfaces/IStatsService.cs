using OSPhoto.Common.Services.Models;

namespace OSPhoto.Common.Interfaces;

public interface IStatsService
{
    Task<StatsResponse> Get();
}
