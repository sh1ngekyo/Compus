namespace Compus.Domain.Client;

/// <summary>
/// Connected session
/// </summary>
public class ExternalActiveSession
{
    /// <summary>
    /// Current stored session
    /// </summary>
    public ExternalStoredSession? StoredSession { get; set; }

    /// <summary>
    /// Id for connection
    /// </summary>
    public Guid ConnectionId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Connection status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Start connection time
    /// </summary>
    public DateTime StartSessionDate { get; set; }
}
