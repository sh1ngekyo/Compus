using Compus.Application.Abstractions;
using SixLabors.Fonts;
using SixLabors.ImageSharp.Drawing.Processing;

namespace Compus.Application.Services.Captcha;

public class CaptchaService : ICaptchaService
{
    private const int MinBrightnessRgb = 160;
    private const string FontName = "Consolas";
    private const string Numbers = "23456789";
    private const string Alphabet = "ABCDEFGHJKLMNPQRTUVWXYZ";

    /// <summary>
    /// Glitches on the output image
    /// </summary>
    private readonly (int Min, int Max) _glitchesCount = (3, 5);
    private readonly Random _random = new();

    public const int CaptchaLength = 4;

    public readonly char[] Characters = (Numbers + Alphabet + Alphabet.ToLower()).ToCharArray();

    public string Generate(int width, int height, out byte[] captchaImages)
    {
        var captcha = string.Empty;
        for (var i = 0; i < CaptchaLength; i++)
        {
            captcha += Characters[_random.Next(0, Characters.Length)];
        }

        captchaImages = GenerateCaptchaImage(width, height, captcha);

        return captcha;
    }

    private byte[] GenerateCaptchaImage(int width, int height, string captchaCode)
    {
        var fontSize = GetFontSize(width, captchaCode.Length);
        var fondFamily = SystemFonts.Collection.Families.FirstOrDefault(u => u.Name == FontName);
        fondFamily = fondFamily == default ? SystemFonts.Collection.Families.Last() : fondFamily;
        var font = SystemFonts.CreateFont(fondFamily.Name, fontSize);

        using var image = new Image<Rgba32>(width, height, GetRandomLightColor());
        DrawCaptchaCode(height, captchaCode, fontSize, font, image);
        DrawGlitches(width, height, image);

        using var ms = new MemoryStream();
        image.SaveAsPng(ms);
        return ms.ToArray();
    }

    private void DrawCaptchaCode(int height, string captchaCode, int fontSize, Font font, Image<Rgba32> image)
    {
        for (int i = 0; i < captchaCode.Length; i++)
        {
            var shiftPx = fontSize / 6;
            var x = _random.Next(-shiftPx, shiftPx) + _random.Next(-shiftPx, shiftPx);
            if (x < 0 && i == 0)
            {
                x = 0;
            }

            x += i * fontSize;

            var maxY = height - fontSize;
            if (maxY < 0)
            {
                maxY = 0;
            }

            var y = _random.Next(0, maxY);

            image.Mutate(operation => 
                operation.DrawText(captchaCode[i].ToString(),
                font,
                GetRandomDeepColor(),
                new PointF(x, y)));
        }
    }

    private Color GetRandomDeepColor() => Color.FromRgb(
            (byte)_random.Next(MinBrightnessRgb),
            (byte)_random.Next(MinBrightnessRgb),
            (byte)_random.Next(MinBrightnessRgb));

    private Color GetRandomLightColor()
    {
        const int low = 200;
        const int high = 255;

        var nRend = _random.Next(high) % (high - low) + low;
        var nGreen = _random.Next(high) % (high - low) + low;
        var nBlue = _random.Next(high) % (high - low) + low;

        return Color.FromRgb(
            (byte)nRend,
            (byte)nGreen,
            (byte)nBlue);
    }

    private int GetFontSize(int imageWidth, int captchCodeCount)
    {
        var averageSize = imageWidth / captchCodeCount;

        return Convert.ToInt32(averageSize);
    }

    private void DrawGlitches(int width, int height, Image graphics)
    {
        for (int i = 0; i < _random.Next(_glitchesCount.Min, _glitchesCount.Max); i++)
        {
            var linePen = new Pen(new SolidBrush(GetRandomLightColor()), 3);
            var startPoint = new Point(_random.Next(0, width), _random.Next(0, height));
            var endPoint = new Point(_random.Next(0, width), _random.Next(0, height));
            graphics.Mutate(operation => operation.DrawLines(linePen, startPoint, endPoint));
        }
    }
}
