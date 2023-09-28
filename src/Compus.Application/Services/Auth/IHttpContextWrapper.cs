using System.Security.Claims;
using Compus.Application.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Compus.Application.Services.Auth;

public class HttpContextWrapper : IHttpContextWrapper
{
    private readonly IHttpContextAccessor _contextAccessor;

    public HttpContextWrapper(IHttpContextAccessor contextAccessor) => _contextAccessor = contextAccessor;

    public bool IsAuthorized() => _contextAccessor.HttpContext.User.Identity!.IsAuthenticated;

    public async Task SignInAsync(string scheme, ClaimsPrincipal claimsPrincipal, AuthenticationProperties properties) 
        => await _contextAccessor.HttpContext.SignInAsync(scheme, claimsPrincipal, properties);

    public async Task SignOutAsync(string scheme) 
        => await _contextAccessor.HttpContext.SignOutAsync(scheme);
}
