using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Compus.Application.Abstractions;

public interface IHttpContextWrapper
{
    bool IsAuthenticated();
    Task SignOutAsync(string scheme); 
    Task SignInAsync(string scheme, ClaimsPrincipal claimsPrincipal, AuthenticationProperties properties);
}
