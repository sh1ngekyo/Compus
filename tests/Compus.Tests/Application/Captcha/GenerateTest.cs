using Compus.Application.Services.Captcha;
using SixLabors.ImageSharp;

namespace Compus.Tests.Application.Captcha;
public class GenerateTest
{
    [Fact(Skip = "Temporary, sixlabors issue")]
    public void GenerateCaptcha_LengthShouldBeEqualsServiceLength()
    {
        var captchaService = new CaptchaService();

        var captcha = captchaService.Generate(200, 60, out var image);

        Assert.Equal(CaptchaService.CaptchaLength, captcha.Length);
    }

    [Fact(Skip = "Temporary, sixlabors issue")]
    public void GenerateCaptcha_ShouldContainsOnlyAlphaNumericChars()
    {
        var captchaService = new CaptchaService();

        var captcha = captchaService.Generate(200, 60, out var image);

        foreach (var ch in captcha)
            Assert.Contains(ch, captchaService.Characters);
    }

    [Fact(Skip = "Temporary, sixlabors issue")]
    public void GenerateCaptcha_ImageSizeShouldBeEqualsExpectedSize()
    {
        var width = 200;
        var height = 60;

        var captchaService = new CaptchaService();

        var captcha = captchaService.Generate(width, height, out var rawImage);

        using var ms = new MemoryStream(rawImage);
        var image = Image.Load(ms);
        Assert.Equal(width, image.Width);
        Assert.Equal(height, image.Height);
    }
}
