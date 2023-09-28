using Compus.Application.Abstractions;
using Compus.Application.Extensions;
using Compus.Domain.Server;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Compus.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly ServerConfig _cfg;
    private readonly IHttpContextWrapper _httpContextWrapper;
    public bool Authorized => _httpContextWrapper.IsAuthorized();

    public AuthService(IHttpContextWrapper httpContextWrapper, ServerConfig cfg)
        => (_cfg, _httpContextWrapper) = (cfg, httpContextWrapper);

    private bool VerifyCaptcha(string receivedCaptcha, string validCaptchaForCurrentSession)
        => !string.IsNullOrWhiteSpace(receivedCaptcha) && 
            validCaptchaForCurrentSession == receivedCaptcha.ToLowerInvariant();

    public bool ValidateCaptcha(string receivedCaptcha, string validCaptchaForCurrentSession) 
        => !_cfg.EnableAuthorization ||
            VerifyCaptcha(receivedCaptcha, validCaptchaForCurrentSession);

    public async Task<bool> SignInAsync(string username, string password, bool persist)
    {
        // skip auth if EnableAuthorization is false
        var user = _cfg.Users!.FirstOrDefault(
            u => !_cfg.EnableAuthorization ||
            (u.UserName == username && u.Password == password));

        if (user == null)
        {
            return false;
        }

        var authProps = user.GetAuthProperties(persist);
        await _httpContextWrapper.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            authProps.Principal,
            authProps.Properties);

        return true;
    }

    public async Task SignOutAsync() 
        => await _httpContextWrapper.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
}
