﻿using Compus.Application.Abstractions;
using Compus.Domain.Server;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Compus.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ServerConfig _shellConfiguration;
    private readonly IHttpContextWrapper _httpContextWrapper;
    public bool Authenticated => _httpContextWrapper.IsAuthenticated();

    public AuthService(IHttpContextWrapper httpContextWrapper, ServerConfig shellConfiguration)
        => (_shellConfiguration, _httpContextWrapper) = (shellConfiguration, httpContextWrapper);

    private bool VerifyCaptcha(string receivedCaptcha, string validCaptchaForCurrentSession)
        => !string.IsNullOrWhiteSpace(receivedCaptcha)
        && validCaptchaForCurrentSession == receivedCaptcha.ToLowerInvariant();

    public bool ValidateCaptcha(string receivedCaptcha, string validCaptchaForCurrentSession) 
        => !_shellConfiguration.EnableAuthorization || VerifyCaptcha(receivedCaptcha, validCaptchaForCurrentSession);

    private (ClaimsPrincipal Principal, AuthenticationProperties Properties) CreateAuthInfo(User user, bool persist)
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

    public async Task<bool> SignInAsync(string username, string password, bool persist)
    {
        var user = _shellConfiguration.Users!.FirstOrDefault(u => !_shellConfiguration.EnableAuthorization || (u.UserName == username && u.Password == password));

        if (user == null)
        {
            return false;
        }
        var authInfo = CreateAuthInfo(user, persist);
        await _httpContextWrapper.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authInfo.Principal, authInfo.Properties);
        return true;
    }

    public async Task SignOutAsync() 
        => await _httpContextWrapper.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
