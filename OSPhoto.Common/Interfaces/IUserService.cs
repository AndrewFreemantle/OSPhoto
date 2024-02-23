namespace OSPhoto.Common.Interfaces;

public interface IUserService
{
    /// <summary>
    /// Verifies the given credentials, and if valid, returns a SessionId
    /// </summary>
    /// <param name="username"></param>
    /// <param name="plainTextPassword"></param>
    /// <returns>SessionId</returns>
    public Task<string> LoginAsync(string username, string plainTextPassword);

    // public void Logout(string sessionId);

    /// <summary>
    /// Validates the given SessionId as being current. Presently, the only way to invalidate a Session is to Logout
    /// </summary>
    /// <param name="sessionId"></param>
    public Task<bool> IsSessionIdValidAsync(string sessionId);

    /// <summary>
    /// Ends the session for the given SessionId
    /// </summary>
    /// <param name="sessionId"></param>
    /// <returns></returns>
    public Task LogoutAsync(string? sessionId);
}
