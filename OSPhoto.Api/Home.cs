using System.Reflection;

namespace OSPhoto.Api;

public class Home : EndpointWithoutRequest
{
    private Assembly _assembly = Assembly.GetExecutingAssembly();
    private string _page = "OSPhoto.Api.Views.home.html";


    public override void Configure()
    {
        Get("/");
        RoutePrefixOverride(string.Empty);
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        using (var stream = _assembly.GetManifestResourceStream(_page))
        {
            using (var reader = new StreamReader(stream))
            {
                var content = await reader.ReadToEndAsync();
                await SendStringAsync(content, 200, "text/html", ct);
            }
        }
    }
}
