namespace Application.Abstractions;

public interface IAuthService
{
    public bool Authenticated { get; }

    public bool ValidateCaptcha(string receivedCaptcha, string validCaptchaForCurrentSession);

    public Task<bool> SignInAsync(string username, string password, bool persist = false);

    public Task SignOutAsync();
}
