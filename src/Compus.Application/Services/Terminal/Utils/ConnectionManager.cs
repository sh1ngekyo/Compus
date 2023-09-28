using System.Collections.Concurrent;
using System.Text;
using Compus.Application.Exceptions;
using Compus.Application.Extensions;
using Compus.Domain.Client;
using Compus.Domain.Server;
using Compus.Domain.Shared;

namespace Compus.Application.Services.Terminal.Utils;

/// <summary>
/// Background worker for connection managment
/// </summary>
public class ConnectionManager
{
    private static ConcurrentDictionary<string, InternalSessionStorage> ConnectionPool { get; set; } = new();
    private readonly ServerConfig _config;

    /// <summary>
    /// Expired connection check interval in ms
    /// </summary>
    private const int ConnectionCheckIntervalMs = 120 * 1000;
    private const string ControlCommand = "ctrl + ";

    public ConnectionManager(ServerConfig config)
    {
        _config = config;
        StartConnectionChecking();
    }

    private void StartConnectionChecking()
        => Task.Run(() =>
           {
               while (true)
               {
                   Thread.Sleep(ConnectionCheckIntervalMs);
                   RemoveExpiredSessions();
               }
           });

    /// <summary>
    /// Remove all expired session for all authorized users
    /// </summary>
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

    /// <summary>
    /// Get session from storage
    /// </summary>
    private bool TryGetSessionByStorageId(string storageId, Guid sessionId, out InternalActiveSession session)
    {
        session = null!;
        return ConnectionPool.TryGetValue(storageId, out var storage) &&
               storage.Sessions.TryGetValue(sessionId, out session!);
    }

    /// <summary>
    /// Add connection to pool and connect to remote terminal
    /// </summary>
    public void AddConnection(string storageId, ExternalActiveSession session)
    {
        if (!ConnectionPool.TryGetValue(storageId, out var sessionsModel))
        {
            sessionsModel = new InternalSessionStorage();
            ConnectionPool.TryAdd(storageId, sessionsModel);
        }

        sessionsModel.Connect(session, _config);
    }

    /// <summary>
    /// Send command to remote terminal
    /// </summary>
    /// <param name="command">Command to send</param>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <exception cref="NoСonnectedTerminalAvailableException"></exception>
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
            return;
        }
        throw new NoСonnectedTerminalAvailableException(sessionId.ToString(), storageId);
    }

    /// <summary>
    /// Get terminal viewport data
    /// </summary>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <exception cref="NoСonnectedTerminalAvailableException"></exception>
    /// <returns>Terminal viewport data from remote server</returns>
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
        throw new NoСonnectedTerminalAvailableException(sessionId.ToString(), storageId);
    }

    /// <summary>
    /// Get connection status for current session
    /// </summary>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <exception cref="NoСonnectedTerminalAvailableException"></exception>
    public bool IsConnected(string storageId, Guid sessionId)
    {
        if (TryGetSessionByStorageId(storageId, sessionId, out var session))
        {
            return session.SshClient!.IsConnected;
        }
        throw new NoСonnectedTerminalAvailableException(sessionId.ToString(), storageId);
    }

    /// <summary>
    /// Disconnect from remote terminal
    /// </summary>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <exception cref="NoСonnectedTerminalAvailableException"></exception>
    public bool Disconnect(string storageId, Guid sessionId)
    {
        if (ConnectionPool.TryGetValue(storageId, out var storage))
        {
            storage.RemoveActiveSession(sessionId);

            return true;
        }
        throw new NoСonnectedTerminalAvailableException(sessionId.ToString(), storageId);
    }

    /// <summary>
    /// Flush connection pool
    /// </summary>
    /// <param name="storageId">Id for current session's storage</param>
    /// <returns>List with connected sessions</returns>
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
