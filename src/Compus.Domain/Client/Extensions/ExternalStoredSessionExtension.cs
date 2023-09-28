using System.Text;

namespace Compus.Domain.Client.Extensions;

/// <summary>
/// ExternalStoredSession
/// </summary>
internal static class ExternalStoredSessionExtension
{
    /// <summary>
    /// Convert from base64 to string
    /// </summary>
    /// <param name="session">Session with password for decode</param>
    /// <returns>Decoded string or string.Empty if any errors occurred</returns>
    internal static string DecodePassword(this ExternalStoredSession session)
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

    /// <summary>
    /// Convert from string to base64
    /// </summary>
    /// <param name="session">Session with password for encode</param>
    /// <param name="value">Password for encode</param>
    /// <returns>Encoded string or string.Empty if any errors occurred</returns>
    internal static string EncodePassword(this ExternalStoredSession session, string? value)
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
