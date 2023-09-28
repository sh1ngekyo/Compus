namespace Compus.Application.Abstractions;

/// <summary>
/// Authorization service
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Indicates if user is authorized
    /// </summary>
    public bool Authorized { get; }

    /// <summary>
    /// Captcha validation
    /// </summary>
    /// <param name="receivedCaptcha">User's captcha response</param>
    /// <param name="validCaptchaForCurrentSession">Correct solved captcha for current session</param>
    /// <returns>True if captcha is valid, otherwise false</returns>
    public bool ValidateCaptcha(string receivedCaptcha, string validCaptchaForCurrentSession);

    /// <summary>
    /// Authorize by username and password
    /// </summary>
    /// <param name="username">UserName</param>
    /// <param name="password">Password</param>
    /// <param name="persist">Save current user</param>
    /// <returns>True if authorized, otherwise false</returns>
    public Task<bool> SignInAsync(string username, string password, bool persist = false);

    /// <summary>
    /// Sign out for current user
    /// </summary>
    public Task SignOutAsync();
}
