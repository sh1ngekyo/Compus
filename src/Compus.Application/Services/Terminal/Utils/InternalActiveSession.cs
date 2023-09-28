using System.Collections.Concurrent;
using Compus.Domain.Client;
using Renci.SshNet;

namespace Compus.Application.Services.Terminal.Utils;

/// <summary>
/// Internal session for connection manager
/// </summary>
public class InternalActiveSession : IDisposable
{
    /// <summary>
    /// Connection Id
    /// </summary>
    public Guid SessionId { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Connection status
    /// </summary>
    public string? Status { get; set; }

    /// <summary>
    /// Start date
    /// </summary>
    public DateTime StartSessionDate { get; set; }

    /// <summary>
    /// Last active date
    /// </summary>
    public DateTime LastActiveSessionDate { get; set; }

    /// <summary>
    /// Contains stored client session
    /// </summary>
    public ExternalStoredSession? StoredSession { get; set; }
    public SshClient? SshClient { get; set; }
    public ShellStream? ShellStream { get; set; }

    /// <summary>
    /// Connection message bus
    /// </summary>
    public ConcurrentQueue<string>? OutputQueue { get; set; }

    public void Dispose()
    {
        ShellStream?.Dispose();
        SshClient?.Dispose();
        ShellStream = null;
        SshClient = null;
    }
}
