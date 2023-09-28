using System.Text;

namespace Compus.Domain.Client.Extensions;

internal static class ExternalStoredSessionExtension
{
    internal static string EncodePassword(this ExternalStoredSession session)
    {
        try
        {
            if (!string.IsNullOrEmpty(session.Password))
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(session.Password));
            }
        }
        catch
        {
            // ignore cuz we don't care about input from client
            // if it's not valid, then it's empty
        }
        return string.Empty;
    }

    internal static string DecodePassword(this ExternalStoredSession session, string? value)
    {
        try
        {
            if (!string.IsNullOrEmpty(value))
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
            }
        }
        catch
        {
            // ignore cuz we don't care about input from client
            // if it's not valid, then it's empty
        }
        return string.Empty;
    }
}
