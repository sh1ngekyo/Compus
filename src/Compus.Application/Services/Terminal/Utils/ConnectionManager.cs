using System.Collections.Concurrent;
using System.Text;
using Compus.Application.Extensions;
using Compus.Domain.Client;
using Compus.Domain.Server;
using Compus.Domain.Shared;

namespace Compus.Application.Services.Terminal.Utils;

public class ConnectionManager
{
    private static ConcurrentDictionary<string, InternalSessionStorage> ConnectionPool { get; set; } = new();
    private readonly ServerConfig _config;
    private const int SessionManagementDelayMs = 120 * 1000;
    private const string ControlCommand = "ctrl + ";

    public ConnectionManager(ServerConfig config)
    {
        _config = config;
        StartSessionManagement();
    }

    private void StartSessionManagement()
        => Task.Run(() =>
           {
               while (true)
               {
                   Thread.Sleep(SessionManagementDelayMs);
                   RemoveExpiredSessions();
               }
           });

    private void RemoveExpiredSessions()
    {
        foreach (var sessionStorage in ConnectionPool.ToArray())
        {
            var expiredSessions = sessionStorage.Value.GetExpiredSessions(_config).ToArray();
            foreach (var expiredSession in expiredSessions)
            {
                sessionStorage.Value.RemoveActiveSession(expiredSession.Key);
            }
            if (sessionStorage.Value.Sessions.IsEmpty)
            {
                ConnectionPool.TryRemove(sessionStorage.Key, out _);
            }
        }
    }

    private bool TryGetSessionByStorageId(string storageId, Guid sessionId, out InternalActiveSession session)
    {
        session = null!;
        return ConnectionPool.TryGetValue(storageId, out var storage) &&
               storage.Sessions.TryGetValue(sessionId, out session!);
    }

    public void AddConnection(string storageId, ExternalActiveSession activeSessionModel)
    {
        if (!ConnectionPool.TryGetValue(storageId, out var sessionsModel))
        {
            sessionsModel = new InternalSessionStorage();
            ConnectionPool.TryAdd(storageId, sessionsModel);
        }

        sessionsModel.Connect(activeSessionModel, _config);
    }

    public void ExecuteCommand(string storageId, Guid sessionId, string command)
    {
        if (TryGetSessionByStorageId(storageId, sessionId, out var session))
        {
            if (command?.StartsWith(ControlCommand) ?? false)
            {
                var lastWord = command[ControlCommand.Length..].ToLower();
                if (lastWord.Length == 1 && lastWord[0] >= 'a' && lastWord[0] <= 'z')
                {
                    command = ((char)(lastWord[0] - 96)).ToString();
                }
            }
            session.LastActiveSessionDate = DateTime.Now;
            session.ShellStream!.WriteLine(command);
        }
        else
        {
            throw new Exception("No available terminal connected");
        }
    }

    public TerminalContent GetTerminalOutput(string storageId, Guid sessionId)
    {
        if (TryGetSessionByStorageId(storageId, sessionId, out var session))
        {
            var totalLines = 0;
            var outputStringBuilder = new StringBuilder();
            while (totalLines < Constants.MaxinumLines && session.OutputQueue!.TryDequeue(out var output))
            {
                totalLines++;
                outputStringBuilder.Append(output);
            }
            return new TerminalContent
            {
                Content = outputStringBuilder?.ToString() ?? string.Empty,
                Lines = totalLines
            };
        }
        else
        {
            throw new Exception("No available terminal connected");
        }
    }

    public bool IsConnected(string storageId, Guid sessionId)
    {
        if (TryGetSessionByStorageId(storageId, sessionId, out var session))
        {
            return session.SshClient!.IsConnected;
        }
        else
        {
            throw new Exception("No available shell connected");
        }
    }

    public bool Disconnect(string storageId, Guid sessionId)
    {
        if (ConnectionPool.TryGetValue(storageId, out var storage))
        {
            storage.RemoveActiveSession(sessionId);

            return true;
        }
        else
        {
            throw new Exception("No available shell connected");
        }
    }

    public List<ExternalActiveSession> FlushStorage(string storageId)
    {
        if (ConnectionPool.TryGetValue(storageId, out var storage))
        {
            var result = new List<ExternalActiveSession>();

            var sessions = storage.Sessions.ToArray();
            foreach (var session in sessions)
            {
                if (!session.Value.SshClient!.IsConnected)
                {
                    storage.RemoveActiveSession(session.Key);
                }
                else
                {
                    result.Add(new ExternalActiveSession
                    {
                        StartSessionDate = session.Value.StartSessionDate,
                        Status = session.Value.Status,
                        StoredSession = session.Value.StoredSession,
                        ConnectionId = session.Key
                    });
                }
            }

            return result;
        }
        return new List<ExternalActiveSession>();
    }
}
