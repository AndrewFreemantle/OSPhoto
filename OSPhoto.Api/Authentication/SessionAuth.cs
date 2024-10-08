using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using OSPhoto.Common.Interfaces;

namespace OSPhoto.Api.Authentication;

// source: https://gist.github.com/dj-nitehawk/ef60db792a56afc23537238e79257d13
public sealed class SessionAuth : AuthenticationHandler<AuthenticationSchemeOptions>
{
    internal const string SchemeName = "Session";
    public const string SessionPropertyName = "PHPSESSID";

    private IUserService _userService;

    public SessionAuth(
        IOptionsMonitor<AuthenticationSchemeOptions> options
        , IUserService userService
        , ILoggerFactory logger
        , UrlEncoder encoder) : base(options, logger, encoder)
    {
        _userService = userService;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (IsPublicEndpoint())
            return await Task.FromResult(AuthenticateResult.NoResult());

        var sessionId = await GetSessionIdFromRequestAsync();
        if (!await _userService.IsSessionIdValidAsync(sessionId))
            return await Task.FromResult(AuthenticateResult.Fail("Invalid Session"));

        return await Task.FromResult(AuthenticateResult.Success(await CreateAuthTicketAsync(sessionId)));
    }

    private bool IsPublicEndpoint() => Context
        .GetEndpoint()?
        .Metadata.OfType<AllowAnonymousAttribute>()
        .Any() is null or true;

    private async Task<string> GetSessionIdFromRequestAsync()
    {
        // try query-string first
        var sessionId = Context.Request.Query.ContainsKey(SessionPropertyName)
            ? Context.Request.Query[SessionPropertyName].ToString()
            : string.Empty;

        if (!string.IsNullOrEmpty(sessionId))
            return sessionId;

        // try cookies
        sessionId = Context.Request.Cookies.ContainsKey(SessionPropertyName)
            ? Context.Request.Cookies[SessionPropertyName]
            : string.Empty;

        if (!string.IsNullOrEmpty(sessionId))
            return sessionId;

        // finally try the request body
        if (sessionId == string.Empty)
        {
            try
            {
                Context.Request.EnableBuffering();
                var originalBodyStream = Context.Request.Body;
                var requestBodyStream = new MemoryStream();
                await originalBodyStream.CopyToAsync(requestBodyStream);
                requestBodyStream.Seek(0, SeekOrigin.Begin);
                using (var reader = new StreamReader(requestBodyStream, Encoding.UTF8))
                {
                    string body = await reader.ReadToEndAsync();
                    if (body.Contains(SessionPropertyName))
                        sessionId = body
                            .Split('&')
                            .FirstOrDefault(s => s.StartsWith(SessionPropertyName), $"{SessionPropertyName}=")
                            .Split('=')
                            .LastOrDefault(string.Empty);
                }

                originalBodyStream.Seek(0, SeekOrigin.Begin);
                Context.Request.Body = originalBodyStream;

                if (sessionId != string.Empty)
                    Logger.Log(LogLevel.Information, "Found SessionId in Body");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Authentication exception reading body for the session identifier");
            }
        }
        else
            Logger.Log(LogLevel.Information, "Found SessionId in Query Parameters");

        return sessionId;
    }

    private async Task<AuthenticationTicket> CreateAuthTicketAsync(string sessionId)
    {
        // TODO: retrieve the user's claims/roles/permissions from a db/cache/env
        var identity = new ClaimsIdentity(claims: new[]
        {
            new Claim("id", "001"),
            new Claim("permissions", "browse"),
            new Claim("permissions", "upload"),
            new Claim("permissions", "manage"),
        }, authenticationType: SchemeName);
        var roles = Array.Empty<string>(); // new[] { "SomeRoleName" };
        var principle = new GenericPrincipal(identity, roles);
        var ticket = new AuthenticationTicket(principle, SchemeName);

        return await Task.FromResult(ticket);
    }
}
