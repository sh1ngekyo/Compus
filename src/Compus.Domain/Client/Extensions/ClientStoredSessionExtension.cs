using System.Text;

namespace Compus.Domain.Client.Extensions;

public static class ClientStoredSessionExtension
{
    public static string DecryptPassword(this ClientStoredSession Session, string password)
    {
        if (!string.IsNullOrEmpty(password))
        {
            try
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(password));
            }
            catch
            {
            }
        }
        return string.Empty;
    }

    public static string EncryptPassword(this ClientStoredSession Session, string password)
        => Session.Password = !string.IsNullOrEmpty(password) ?
            Convert.ToBase64String(Encoding.UTF8.GetBytes(password))
            : string.Empty;

    public static ClientStoredSession Clone(this ClientStoredSession Session, bool sameKey = true) => new()
    {
        Id = sameKey ? Session.Id : Guid.NewGuid(),
        DisplayName = Session.DisplayName,
        Host = Session.Host,
        Port = Session.Port,
        UserName = Session.UserName,
        Password = Session.Password,
        FingerPrint = Session.FingerPrint,
        LoginKey = Session.LoginKey,
    };
}
