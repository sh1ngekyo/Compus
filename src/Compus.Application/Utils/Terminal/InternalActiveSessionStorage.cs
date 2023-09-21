using System.Collections.Concurrent;
using System.Text;
using Compus.Domain.Client.Extensions;
using Compus.Domain.Server;
using Compus.Domain.Shared;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace Compus.Application.Utils.Terminal;

public class InternalActiveSessionStorage
{
    public ConcurrentDictionary<Guid, InternalActiveSession> Sessions { get; set; } = new();

    public InternalActiveSession Connect(Domain.Client.ExternalActiveSession activeSession, ServerConfig serverConfig)
    {
        SshClient sshClient = default!;
        ShellStream shellStream = default!;
        try
        {
            var timeOutMinutes = serverConfig.MaxIdleMinutes < 1
                ? 1
                : serverConfig.MaxIdleMinutes > 20 ? 20
                : serverConfig.MaxIdleMinutes;
            var clientStoredSessionModel = activeSession.StoredSession!;

            sshClient = new SshClient(
                clientStoredSessionModel.Host,
                clientStoredSessionModel.Port,
                clientStoredSessionModel.UserName,
                clientStoredSessionModel.DecryptPassword(
                    clientStoredSessionModel.Password!));

            sshClient.ConnectionInfo.Timeout = TimeSpan.FromMinutes(timeOutMinutes);
            sshClient.Connect();
            shellStream = sshClient.CreateShellStream("Terminal", 80, 30, 800, 400, 1000);

            var outputQueue = new ConcurrentQueue<string>();

            var sessionModel = new InternalActiveSession
            {
                Status = "Connected",
                Id = activeSession.Id,
                ShellStream = shellStream,
                Client = sshClient,
                CreatedDate = DateTime.Now,
                LastActiveDate = DateTime.Now,
                StoredClientSession = clientStoredSessionModel,
                OutputQueue = outputQueue
            };

            string result = string.Empty;
            while ((result = sessionModel.ShellStream.ReadLine(TimeSpan.FromSeconds(0.3))) != null)
            {
                outputQueue.Enqueue(result + Constants.NewLineForShell);
            }

            outputQueue.Enqueue(sessionModel.ShellStream.Read());

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

            Sessions.TryAdd(sessionModel.Id, sessionModel);

            sshClient.ErrorOccurred += OnErrorOccurred!;

            return sessionModel;
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
        if (Sessions.TryRemove((sender as InternalActiveSession)?.Id ?? Guid.Empty, out var activeSession))
        {
            activeSession.Dispose();
        }
    }
}
