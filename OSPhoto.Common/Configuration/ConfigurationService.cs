using Microsoft.Extensions.Options;
using OSPhoto.Common.Configuration;

public class ConfigurationService : IConfigurationService
{
    public ConfigurationService(IOptions<AppSettings> options)
    {
        Settings = options.Value;
    }

    public AppSettings Settings { get; }
}
