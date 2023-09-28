using System.Collections.Concurrent;
using System.Text;
using Compus.Domain.Client;
using Compus.Domain.Server;
using Compus.Domain.Shared;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Compus.Application.Services.Terminal.Utils;

/// <summary>
/// Internal session storage for connection manager
/// </summary>
public class InternalSessionStorage
{
    private const int TerminalBufferSize = 1000;
    private const uint TerminalRows = 30;
    private const uint TerminalCols = 80;

    private readonly TimeSpan _terminalTimeout = TimeSpan.FromSeconds(0.3);
    private readonly (uint Width, uint Height) _terminalSize = (800, 400);

    public ConcurrentDictionary<Guid, InternalActiveSession> Sessions { get; set; } = new();

    /// <summary>
    /// Connect to remote terminal
    /// </summary>
    public InternalActiveSession Connect(ExternalActiveSession clientSession, ServerConfig config)
    {
        SshClient sshClient = null!;
        ShellStream shellStream = null!;
        try
        {
            var timeOutMinutes = config.MaxIdleMinutes < 1 ? 1 : config.MaxIdleMinutes > 20 ? 20 : config.MaxIdleMinutes;
            var clientStoredSession = clientSession.StoredSession;

            sshClient = new SshClient(clientStoredSession!.Host,
                clientStoredSession.Port,
                clientStoredSession.UserName,
                clientStoredSession.DecryptedPassword);

            sshClient.ConnectionInfo.Timeout = TimeSpan.FromMinutes(timeOutMinutes);
            sshClient.Connect();

            shellStream = sshClient.CreateShellStream(
                "Terminal",
                TerminalCols,
                TerminalRows,
                _terminalSize.Width,
                _terminalSize.Height,
                TerminalBufferSize);

            var outputQueue = new ConcurrentQueue<string>();
            var session = new InternalActiveSession
            {
                Status = "Connected",
                SessionId = clientSession.ConnectionId,
                ShellStream = shellStream,
                SshClient = sshClient,
                StartSessionDate = DateTime.Now,
                LastActiveSessionDate = DateTime.Now,
                StoredSession = clientStoredSession,
                OutputQueue = outputQueue
            };

            string result = null!;
            while ((result = session.ShellStream.ReadLine(_terminalTimeout)) != null)
            {
                outputQueue.Enqueue(result + Constants.NewLine);
            }

            outputQueue.Enqueue(session.ShellStream.Read());

            shellStream.DataReceived += (obj, e) =>
            {
                try
                {
                    outputQueue.Enqueue(Encoding.UTF8.GetString(e.Data));

                    if (outputQueue.Count > Constants.MaxinumQueueCount)
                    {
                        outputQueue.TryDequeue(out _);
                    }
                }
                catch (Exception ex)
                {
                    outputQueue.Enqueue(ex.Message);
                }
            };

            Sessions.TryAdd(session.SessionId, session);

            sshClient.ErrorOccurred += OnErrorOccurred!;

            return session;
        }
        catch
        {
            shellStream?.Dispose();
            sshClient?.Dispose();
            throw;
        }
    }

    private void OnErrorOccurred(object sender, ExceptionEventArgs e)
    {
        RemoveActiveSession((sender as InternalActiveSession)?.SessionId ?? Guid.Empty);
    }

    public void RemoveActiveSession(Guid sessionId)
    {
        if (Sessions.TryRemove(sessionId, out var activeSession))
        {
            activeSession.Dispose();
        }
    }
}
