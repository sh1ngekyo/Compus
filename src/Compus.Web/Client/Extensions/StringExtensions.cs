namespace Compus.Web.Client.Extensions;

public static class StringExtensions
{
    public static string[] SplitByLines(this string value)
        => value?.Replace("\r\n", "\n").Split("\n") ?? Array.Empty<string>();
}
