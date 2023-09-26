using System.Collections.Concurrent;

namespace Compus.Domain.Client;

public class ExternalSessionStorage
{
    public void RemoveActiveSession(Guid sessionId) => Sessions.TryRemove(sessionId, out _);

    public void RemoveActiveSessions() => Sessions.Clear();

    public void AddActiveSession(ExternalActiveSession session) => Sessions.TryAdd(session.ConnectionId, session);

    public ConcurrentDictionary<Guid, ExternalActiveSession> Sessions { get; set; } = new();
}
