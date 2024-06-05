using OSPhoto.Common.Interfaces;
using OSPhoto.Common.Services.Models;

public class Stats(IStatsService service) : EndpointWithoutRequest<StatsResponse>
{
    public override void Configure()
    {
        Get("stats");
        RoutePrefixOverride(string.Empty);
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Logger.LogInformation("Stats");

        try
        {
            await SendAsync(await service.Get());
        }
        catch (Exception e)
        {
            Logger.LogError(e, " > Error getting statistics");
            await SendNoContentAsync();
        }
    }
}
