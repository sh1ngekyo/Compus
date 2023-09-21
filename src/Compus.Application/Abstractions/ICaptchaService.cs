namespace Compus.Application.Abstractions;

public interface ICaptchaService
{
    public string Generate(int width, int height, out byte[] rawImage);
}
