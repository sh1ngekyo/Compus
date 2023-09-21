using Compus.Application.Abstractions;
using Compus.Application.Services.Auth;
using Compus.Domain.Server;

namespace Compus.Tests.Application.Auth;

public class ValidateCaptchaTest
{
    private readonly string _captchaPattern = "test";

    [Fact]
    public void ValidateCaptcha_ShouldReturnTrueWhenValidAndAuthDisabled()
    {
        var config = new ServerConfig
        {
            EnableAuthorization = false,
        };
        var authService = new AuthService(NSubstitute.Substitute.For<IHttpContextWrapper>(), config);
        var captcha = new string(_captchaPattern);

        var result = authService.ValidateCaptcha(captcha, _captchaPattern);

        Assert.True(result);
    }

    [Fact]
    public void ValidateCaptcha_ShouldReturnTrueWhenNotValidAndAuthDisabled()
    {
        var config = new ServerConfig
        {
            EnableAuthorization = false,
        };
        var authService = new AuthService(NSubstitute.Substitute.For<IHttpContextWrapper>(), config);
        var captcha = Guid.NewGuid().ToString();

        var result = authService.ValidateCaptcha(captcha, _captchaPattern);

        Assert.True(result);
    }

    [Fact]
    public void ValidateCaptcha_ShouldReturnFalseWhenNotValidAndAuthEnabled()
    {
        var config = new ServerConfig
        {
            EnableAuthorization = true,
        };
        var authService = new AuthService(NSubstitute.Substitute.For<IHttpContextWrapper>(), config);
        var captcha = Guid.NewGuid().ToString();

        var result = authService.ValidateCaptcha(captcha, _captchaPattern);

        Assert.False(result);
    }
}
