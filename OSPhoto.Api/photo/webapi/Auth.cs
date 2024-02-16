using OSPhoto.Api;

public class AuthRequest : RequestBase
{
    public string Username { get; set; }
    public string Password { get; set; }

    // login.php uses 'action' instead of 'method'
    public string Action { get; set; }
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
    public Permission Permission { get; set; } = new();

    [JsonPropertyName("enable_face_recog")]
    public bool EnableFaceRecog { get; set; } = false;
    [JsonPropertyName("allow_public_share")]
    public bool AllowPublicShare { get; set; } = false;
    [JsonPropertyName("allow_download")]
    public bool AllowDownload { get; set; } = true;
    [JsonPropertyName("show_detail")]
    public bool ShowDetail { get; set; } = true;
}

public class Auth : Endpoint<AuthRequest, AuthResponse>
{
    public override void Configure()
    {
        Post("auth.php");
        // Post(["/photo/mApp/ajax/login.php", "/photo/webapi/auth.php"]);
        // RoutePrefixOverride(string.Empty);
        AllowFormData(urlEncoded: true);
        AllowAnonymous();
    }

    public override async Task HandleAsync(AuthRequest req, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(req.Method))
            req.Method = req.Action;

        Logger.LogInformation("Auth (method: {method})", req.Method);

        switch (req.Method)
        {
            case RequestMethod.Login:
                var res = new AuthResponse(req.Username);
                Logger.LogInformation(" > user: {username}, sid: {sid}"
                    , res.Data.Username
                    , res.Data.Sid);
                await SendAsync(res);
                break;
            case RequestMethod.Logout:
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}"
                    , req.Method);
                await SendNoContentAsync();
                break;
        }
    }
}
