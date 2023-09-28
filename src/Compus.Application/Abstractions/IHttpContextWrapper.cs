using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Compus.Application.Abstractions;

public interface IHttpContextWrapper
{
    /// <summary>
    /// Indicates if user is authorized
    /// </summary>
    bool IsAuthorized();

    /// <summary>
    /// Sign out by scheme
    /// </summary>
    /// <param name="scheme">Authentication scheme</param>
    Task SignOutAsync(string scheme);

    /// <summary>
    /// Authorize by scheme and props
    /// </summary>
    /// <param name="scheme">Authentication scheme</param>
    /// <param name="claimsPrincipal">Claims principal</param>
    /// <param name="properties">Authentication properties</param>
    Task SignInAsync(string scheme, ClaimsPrincipal claimsPrincipal, AuthenticationProperties properties);
}
