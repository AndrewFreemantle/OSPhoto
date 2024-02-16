namespace OSPhoto.Api.Processors;

public class AdditionalResponseHeadersPreProcessor : IGlobalPreProcessor
{
    public Task PreProcessAsync(IPreProcessorContext ctx, CancellationToken ct)
    {
        ctx.HttpContext.Response.Headers.Server = "nginx";
        ctx.HttpContext.Response.Headers.Append("X-Clacks-Overhead", "GNU Sir Terry Pratchett");
        return Task.CompletedTask;
    }
}
