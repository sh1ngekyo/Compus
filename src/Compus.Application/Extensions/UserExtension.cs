using Compus.Domain.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Compus.Application.Extensions;

internal static class UserExtension
{
    /// <summary>
    /// Create ClaimsPrincipal and AuthenticationProperties for current user
    /// </summary>
    /// <param name="user">User</param>
    /// <param name="persist">Remember user</param>
    /// <returns>ClaimsPrincipal and AuthenticationProperties</returns>
    internal static (ClaimsPrincipal Principal, AuthenticationProperties Properties) GetAuthProperties(this User user, bool persist = false)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.UserName!)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);
        var properties = new AuthenticationProperties();

        if (persist)
        {
            properties.IsPersistent = true;
            properties.ExpiresUtc = DateTime.UtcNow.AddMonths(12);
        }

        return (principal, properties);
    }
}
