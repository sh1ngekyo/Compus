namespace Compus.Application.Abstractions;

/// <summary>
/// Captcha generation service
/// </summary>
public interface ICaptchaService
{
    /// <summary>
    /// Generate captcha image
    /// </summary>
    /// <param name="width">Image width</param>
    /// <param name="height">Image height</param>
    /// <param name="rawImage">Image</param>
    /// <returns>Captcha as string</returns>
    public string Generate(int width, int height, out byte[] rawImage);
}
