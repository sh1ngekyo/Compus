using System.Collections.Concurrent;
using Compus.Domain.Client;
using Renci.SshNet;

namespace Compus.Application.Utils.Terminal
{
    public class InternalActiveSession : IDisposable
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastActiveDate { get; set; }
        public ClientStoredSession? StoredClientSession { get; set; }
        public SshClient? Client { get; set; }
        public ShellStream? ShellStream { get; set; }
        public ConcurrentQueue<string>? OutputQueue { get; set; }

        public void Dispose()
        {
            ShellStream?.Dispose();
            Client?.Dispose();
            ShellStream = null;
            Client = null;
        }
    }
}
