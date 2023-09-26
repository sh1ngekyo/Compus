using System.Collections.Concurrent;
using System.Text;
using Compus.Domain.Client;
using Compus.Domain.Server;
using Compus.Domain.Shared;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Compus.Application.Services.Terminal;

public class InternalSessionStorage
{
    public ConcurrentDictionary<Guid, InternalActiveSession> Sessions { get; set; } = new();

    public InternalActiveSession Connect(ExternalActiveSession activeSessionModel, ServerConfig config)
    {
        SshClient sshClient = null!;
        ShellStream shellStream = null!;
        try
        {
            var timeOutMinutes = config.MaxIdleMinutes < 1 ? 1 : config.MaxIdleMinutes > 20 ? 20 : config.MaxIdleMinutes;
            var clientStoredSession = activeSessionModel.StoredSession;

            sshClient = new SshClient(clientStoredSession!.Host,
                clientStoredSession.Port,
                clientStoredSession.UserName,
                clientStoredSession.DecryptedPassword);

            sshClient.ConnectionInfo.Timeout = TimeSpan.FromMinutes(timeOutMinutes);
            sshClient.Connect();
            shellStream = sshClient.CreateShellStream("Terminal", 80, 30, 800, 400, 1000);

            var outputQueue = new ConcurrentQueue<string>();
            var session = new InternalActiveSession
            {
                Status = "Connected",
                SessionId = activeSessionModel.ConnectionId,
                ShellStream = shellStream,
                SshClient = sshClient,
                StartSessionDate = DateTime.Now,
                LastActiveSessionDate = DateTime.Now,
                StoredSession = clientStoredSession,
                OutputQueue = outputQueue
            };

            string result = null!;
            while ((result = session.ShellStream.ReadLine(TimeSpan.FromSeconds(0.3))) != null)
            {
                outputQueue.Enqueue(result + Constants.NewLine);
            }

            outputQueue.Enqueue(session.ShellStream.Read());

            shellStream.DataReceived += (obj, e) =>
            {
                try
                {
                    outputQueue.Enqueue(Encoding.UTF8.GetString(e.Data));

                    if(outputQueue.Count > Constants.MaxinumQueueCount)
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

    void OnErrorOccurred(object sender, ExceptionEventArgs e)
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
