using System.Text;
using System.Text.Json.Serialization;
using Compus.Domain.Client.Extensions;
using Compus.Domain.Shared;

namespace Compus.Domain.Client;

public class ExternalStoredSession
{
    public Guid ConnectionId { get; set; } = Guid.NewGuid();
    public string? Name { get; set; }
    public string? Host { get; set; }
    public int Port { get; set; } = Constants.DefaultPort;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public byte[]? FingerPrint { get; set; }
    public byte[]? LoginKey { get; set; }

    [JsonIgnore]
    public string DecryptedPassword
    {
        get => this.EncodePassword();
        set => Password = this.DecodePassword(value);
    }

    public ExternalStoredSession Clone(bool nonUninqueId = true) => new()
    {
        ConnectionId = nonUninqueId ? ConnectionId : Guid.NewGuid(),
        Name = Name,
        Host = Host,
        Port = Port,
        UserName = UserName,
        Password = Password,
        FingerPrint = FingerPrint,
        LoginKey = LoginKey,
    };
}
