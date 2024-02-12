public class AuthRequest : RequestBase
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class AuthResponse(string username)
{
    public bool Success { get; set; } = true;
    public AuthResponseData Data { get; set; } = new AuthResponseData(username);
}

public class AuthResponseData(string username)
{
    // TODO: Store this in session...
    public string Sid { get; set; } = Guid.NewGuid().ToString().Replace("-", "");
    public string Username { get; set; } = username;

    // Permissions
    [JsonPropertyName("reg_syno_user")]
    public bool RegSynoUser { get; set; } = true;
    [JsonPropertyName("is_admin")]
    public bool IsAdmin { get; set; } = false;
    [JsonPropertyName("allow_comment")]
    public bool AllowComment { get; set; } = false;
    public AuthPermission Permission { get; set; } = new AuthPermission();

    [JsonPropertyName("enable_face_recog")]
    public bool EnableFaceRecog { get; set; } = false;
    [JsonPropertyName("allow_public_share")]
    public bool AllowPublicShare { get; set; } = false;
    [JsonPropertyName("allow_download")]
    public bool AllowDownload { get; set; } = true;
    [JsonPropertyName("show_detail")]
    public bool ShowDetail { get; set; } = true;
}

public class AuthPermission
{
    // these were false...
    public bool Browse { get; set; } = true;
    public bool Upload { get; set; } = true;
    public bool Manage { get; set; } = true;
}

public class Auth : Endpoint<AuthRequest, AuthResponse>
{
    public override void Configure()
    {
        Post("auth.php");
        AllowFormData(urlEncoded: true);
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthRequest req, CancellationToken ct)
    {
        Console.WriteLine($"AUTH CALLED: {req.Method}");

        switch (req.Method.ToLowerInvariant())
        {
            case "login":
                var res = new AuthResponse(req.Username);
                Console.WriteLine($"> {res.Data.Sid}");
                await SendAsync(res);
                break;
            default:
                Console.WriteLine($"> don't know how to handle requested method: {req.Method}");
                await SendNoContentAsync();
                break;
        }
    }
}
