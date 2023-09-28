using Compus.Domain.Server.Enums;

namespace Compus.Domain.Client;

/// <summary>
/// Model for auth
/// </summary>
public class SignInViewModel
{
    /// <summary>
    /// Request Id
    /// </summary>
    public Guid RequestId { get; set; }

    /// <summary>
    /// User Name
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// User Password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Save user info
    /// </summary>
    public bool Persist { get; set; }

    /// <summary>
    /// Solved captcha
    /// </summary>
    public string? Captcha { get; set; }

    /// <summary>
    /// Request status
    /// </summary>
    public SignInStatus Status { get; set; }

    /// <summary>
    /// Message for errors and warnings
    /// </summary>
    public string? ErrorMessage { get; set; }
}
