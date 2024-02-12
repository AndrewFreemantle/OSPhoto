using OSPhoto.Api.Authentication;

namespace OSPhoto.Api.Models;

/// <summary>
/// Common Api Request Base Model
/// </summary>
public class RequestBase
{
    public string Api { get; set; }
    public string Method { get; set; }
    public int Version { get; set; } = 1;
    [JsonPropertyName(SessionAuth.SessionPropertyName)]
    [BindFrom(SessionAuth.SessionPropertyName)]
    public string? SessionId { get; set; }
}
