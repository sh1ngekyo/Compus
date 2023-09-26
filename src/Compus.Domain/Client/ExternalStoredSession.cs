using System.Text;
using System.Text.Json.Serialization;

namespace Compus.Domain.Client;

public class ExternalStoredSession
{
    public Guid ConnectionId { get; set; } = Guid.NewGuid();
    public string? DisplayName { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = 22;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public byte[]? FingerPrint { get; set; }
    public byte[]? LoginKey { get; set; }

    [JsonIgnore]
    public string DecryptedPassword
    {
        get => EncodePassword(Password);
        set => Password = !string.IsNullOrEmpty(value) ? Convert.ToBase64String(Encoding.UTF8.GetBytes(value)) : string.Empty;
    }

    private string EncodePassword(string? password)
    {
        try
        {
            if (!string.IsNullOrEmpty(Password))
            {
                return Encoding.UTF8.GetString(Convert.FromBase64String(Password));
            }
        }
        catch
        {
        }
        return string.Empty;
    }

    public ExternalStoredSession Clone(bool nonUninqueId = true) => new()
    {
        ConnectionId = nonUninqueId ? ConnectionId : Guid.NewGuid(),
        DisplayName = DisplayName,
        Host = Host,
        Port = Port,
        UserName = UserName,
        Password = Password,
        FingerPrint = FingerPrint,
        LoginKey = LoginKey,
    };
}
