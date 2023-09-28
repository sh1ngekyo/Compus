using System.Text.Json.Serialization;
using Compus.Domain.Client.Extensions;
using Compus.Domain.Shared;

namespace Compus.Domain.Client;

/// <summary>
/// Client side stored session
/// </summary>
public class ExternalStoredSession
{
    /// <summary>
    /// Session Id
    /// </summary>
    public Guid ConnectionId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Session Name
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Session host address
    /// </summary>
    public string? Host { get; set; }

    /// <summary>
    /// Port for connection
    /// </summary>
    public int Port { get; set; } = Constants.DefaultPort;

    /// <summary>
    /// User name for connection
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// Password for connection
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// Finger print (unused)
    /// </summary>
    public byte[]? FingerPrint { get; set; }

    /// <summary>
    /// Login token (unused)
    /// </summary>
    public byte[]? LoginKey { get; set; }

    /// <summary>
    /// Decode/Encode password for local storage
    /// </summary>
    [JsonIgnore]
    public string DecryptedPassword
    {
        get => this.DecodePassword();
        set => Password = this.EncodePassword(value);
    }

    /// <summary>
    /// Create new session based on this instance
    /// </summary>
    /// <param name="nonUninqueId">Copy session id if true, otherwise create new guid</param>
    /// <returns>New session object</returns>
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
