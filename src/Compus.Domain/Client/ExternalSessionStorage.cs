using System.Collections.Concurrent;

namespace Compus.Domain.Client;

/// <summary>
/// Storage for connections
/// </summary>
public class ExternalSessionStorage
{
    /// <summary>
    /// Session dictionary
    /// </summary>
    public ConcurrentDictionary<Guid, ExternalActiveSession> Sessions { get; set; } = new();

    /// <summary>
    /// Remove session from storage
    /// </summary>
    /// <param name="sessionId">Session Id for delete</param>
    public void RemoveActiveSession(Guid sessionId) => Sessions.TryRemove(sessionId, out _);

    /// <summary>
    /// Remove all sessions from storage
    /// </summary>
    public void ClearSessions() => Sessions.Clear();

    /// <summary>
    /// Add new session to storage
    /// </summary>
    /// <param name="session">Item to add</param>
    public void AddActiveSession(ExternalActiveSession session) => Sessions.TryAdd(session.ConnectionId, session);
}
