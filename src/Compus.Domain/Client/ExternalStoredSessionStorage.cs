namespace Compus.Domain.Client;

/// <summary>
/// Client side stored session (Local storage)
/// </summary>
public class ExternalStoredSessionStorage
{
    /// <summary>
    /// List of stored sessions
    /// </summary>
    public List<ExternalStoredSession> Sessions { get; set; } = new();

    /// <summary>
    /// Save session to storage
    /// </summary>
    /// <param name="session">Session for update or add</param>
    public void RenewStoredSession(ExternalStoredSession session)
    {
        var exist = Sessions.FirstOrDefault(u => u.ConnectionId == session.ConnectionId);
        if (exist == null)
        {
            Sessions.Add(session);
            return;
        }
        if (!session.Equals(exist))
        {
            Sessions.Remove(exist);
            Sessions.Add(session);
        }
    }

    /// <summary>
    /// Remove session from storage
    /// </summary>
    /// <param name="sessionId">Session Id for delete</param>
    public void RemoveStoredSession(Guid sessionId)
    {
        var session = Sessions.FirstOrDefault(u => u.ConnectionId == sessionId);
        if (session != null)
        {
            Sessions.Remove(session);
        }
    }
}
