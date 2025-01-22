using OSPhoto.Api;
using OSPhoto.Common.Models;

public class AuthRequest : RequestBase
{
    public string Username { get; set; }
    public string Password { get; set; }

    // login.php uses 'action' instead of 'method'
    public string Action { get; set; }
}

public class AuthResponse(bool success)
{
    public bool Success => success;
}

public class AuthResponseFailure() : AuthResponse(false)
{
    public AuthResponseError Error => new();

    public class AuthResponseError
    {
        public int Code => 102;     // 102: invalid username or password
    }
}

public class AuthResponseSuccess(string sessionId, string username) : AuthResponse(true)
{
    public AuthResponseData Data => new (sessionId, username);

    public class AuthResponseData(string sessionId, string username)
    {
        public string Sid => sessionId;
        public string Username => username;

        // Permissions
        [JsonPropertyName("reg_syno_user")]
        public bool RegSynoUser { get; set; } = true;
        [JsonPropertyName("is_admin")]
        public bool IsAdmin { get; set; } = true;

        [JsonPropertyName("allow_comment")]
        public bool AllowComment { get; set; } = !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("ALLOW_COMMENTS"));
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
}

public class Auth(IUserService userService) : Endpoint<AuthRequest, AuthResponse>
{
    public override void Configure()
    {
        Post("auth.php");
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
                var sessionId = await userService.LoginAsync(req.Username, req.Password);
                if (string.IsNullOrEmpty(sessionId))
                    await SendAsync(new AuthResponseFailure());
                else
                {
                    var res = new AuthResponseSuccess(sessionId, req.Username);
                    Logger.LogInformation(" > user: {username}, sid: {sid}"
                        , res.Data.Username
                        , res.Data.Sid);
                    await SendAsync(res);
                }
                break;
            case RequestMethod.Logout:
                await userService.LogoutAsync(req.SessionId);
                Logger.LogInformation(" > sid: {sid} logged out", req.SessionId);
                await SendAsync(new AuthResponse(true));
                break;
            default:
                Logger.LogError(" > don't know how to handle requested method: {method}" +
                                "\n > body: {body}",
                    req.Method,
                    await HttpContext.Request.Body.ReadAsStringAsync());
                await SendNoContentAsync();
                break;
        }
    }
}
