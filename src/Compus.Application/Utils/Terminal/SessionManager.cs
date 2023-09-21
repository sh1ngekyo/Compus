using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Compus.Application.Abstractions;
using Compus.Domain.Server;
using Compus.Domain.Shared;

namespace Compus.Application.Utils.Terminal;

public class SessionManager : ISessionManager
{
    private static ConcurrentDictionary<string, InternalActiveSessionStorage> _storagePool { get; set; } = new();
    public ServerConfig ServerConfig { get; }

    // 2 minutes
    private const int DelayForCheckInMs = 120 * 1000;
    private const string ControlCommand = "ctrl + ";

    public SessionManager(ServerConfig serverConfig)
    {
        ServerConfig = serverConfig;
        Task.Run(() => ManageTerminalPool());
    }

    private void ManageTerminalPool()
    {
        while (true)
        {
            Task.Delay(DelayForCheckInMs);

            foreach (var storage in _storagePool.ToArray())
            {
                var expiredSessions = GetExpiredSessions(storage.Value).ToArray();

                foreach (var session in expiredSessions)
                {
                    storage.Value.Sessions.Remove(session.Key, out _);
                }

                if (storage.Value.Sessions.Count == 0)
                {
                    _storagePool.TryRemove(storage.Key, out _);
                }
            }
        }
    }

    private IEnumerable<KeyValuePair<Guid, InternalActiveSession>> GetExpiredSessions(InternalActiveSessionStorage sessionStorage)
    {
        return sessionStorage.Sessions
            .Where(u => u.Value.LastActiveDate < DateTime.Now.AddMinutes(-ServerConfig.MaxIdleMinutes)
                    || !u.Value.Client!.IsConnected);
    }

    public void CreateSession(string storageId, Domain.Client.ExternalActiveSession activeSession)
    {
        if (!_storagePool.TryGetValue(storageId, out var sessionsStorage))
        {
            sessionsStorage = new InternalActiveSessionStorage();
            _storagePool.TryAdd(storageId, sessionsStorage);
        }

        sessionsStorage.Connect(activeSession, ServerConfig);
    }

    public void SendCommand(string storageId, Guid sessionId, string command)
    {
        if (_storagePool.TryGetValue(storageId, out var serverActiveSessionsModel) && serverActiveSessionsModel.Sessions.TryGetValue(sessionId, out var serverActiveSessionModel))
        {
            if (command?.StartsWith(ControlCommand) ?? false)
            {
                var lastWord = command[ControlCommand.Length..].ToLower();
                if (lastWord.Length == 1 && lastWord[0] >= 'a' && lastWord[0] <= 'z')
                {
                    command = ((char)(lastWord[0] - 96)).ToString();
                }
            }

            serverActiveSessionModel.LastActiveDate = DateTime.Now;
            serverActiveSessionModel.ShellStream!.WriteLine(command);
        }
        throw new Exception("No available terminal connected");
    }

    public TerminalOutput GetTerminalOutput(string storageId, Guid sessionId)
    {
        if (_storagePool.TryGetValue(storageId, out var serverActiveSessionsModel) && serverActiveSessionsModel.Sessions.TryGetValue(sessionId, out var serverActiveSessionModel))
        {
            var totalLines = 0;
            var outputStringBuilder = new StringBuilder();
            while (totalLines < Constants.MaxinumLines && serverActiveSessionModel.OutputQueue!.TryDequeue(out var output))
            {
                totalLines++;
                outputStringBuilder.Append(output);
            }

            return new TerminalOutput { Content = outputStringBuilder?.ToString() ?? string.Empty, Lines = totalLines };
        }
        throw new Exception("No available terminal connected");
    }

    public bool IsConnected(string storageId, Guid sessionId)
    {
        if (_storagePool.TryGetValue(storageId, out var serverActiveSessionsModel))
        {
            if (serverActiveSessionsModel.Sessions.TryGetValue(sessionId, out var serverActiveSessionModel))
            {
                return serverActiveSessionModel.Client!.IsConnected;
            }
        }
        throw new Exception("No available terminal connected");
    }

    public bool Disconnect(string storageId, Guid sessionId)
    {
        if (_storagePool.TryGetValue(storageId, out var serverActiveSessionsModel))
        {
            serverActiveSessionsModel.Sessions.Remove(sessionId, out _);
            return true;
        }
        throw new Exception("No available terminal connected");
    }

    public List<Domain.Client.ExternalActiveSession> Refresh(string storageId)
    {
        if (_storagePool.TryGetValue(storageId, out var serverActiveSessionsModel))
        {
            var result = new List<Domain.Client.ExternalActiveSession>();

            var sessions = serverActiveSessionsModel.Sessions.ToArray();
            foreach (var session in sessions)
            {
                if (!session.Value.Client!.IsConnected)
                {
                    serverActiveSessionsModel.Sessions.Remove(session.Key, out _);
                }
                else
                {
                    result.Add(new Domain.Client.ExternalActiveSession
                    {
                        StartSessionDate = session.Value.CreatedDate,
                        Status = session.Value.Status,
                        StoredSession = session.Value.StoredClientSession,
                        Id = session.Key
                    });
                }
            }

            return result;
        }
        return new List<Domain.Client.ExternalActiveSession>();
    }
}
