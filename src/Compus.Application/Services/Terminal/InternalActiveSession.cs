using System.Collections.Concurrent;
using Compus.Domain.Client;
using Renci.SshNet;

namespace Compus.Application.Services.Terminal;

public class InternalActiveSession : IDisposable
{
    public Guid SessionId { get; set; } = Guid.NewGuid();
    public string? Status { get; set; }
    public DateTime StartSessionDate { get; set; }
    public DateTime LastActiveSessionDate { get; set; }

    public ExternalStoredSession? StoredSession { get; set; }
    public SshClient? SshClient { get; set; }
    public ShellStream? ShellStream { get; set; }
    public ConcurrentQueue<string>? OutputQueue { get; set; }

    public void Dispose()
    {
        ShellStream?.Dispose();
        SshClient?.Dispose();
        ShellStream = null;
        SshClient = null;
    }
}
