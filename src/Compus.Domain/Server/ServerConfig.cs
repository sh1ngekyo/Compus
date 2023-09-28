namespace Compus.Domain.Server;

/// <summary>
/// Temporary configuration for API
/// </summary>
public class ServerConfig
{
    /// <summary>
    /// User storage
    /// </summary>
    public User[]? Users { get; set; }

    /// <summary>
    /// Time until automatic shutdown when inactive
    /// </summary>
    public int MaxIdleMinutes { get; set; }

    /// <summary>
    /// Authorization enabling
    /// </summary>
    public bool EnableAuthorization { get; set; }
}
