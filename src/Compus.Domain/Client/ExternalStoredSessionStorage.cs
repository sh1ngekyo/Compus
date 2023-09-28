namespace Compus.Domain.Client;

public class ExternalStoredSessionStorage
{
    public List<ExternalStoredSession> Sessions { get; set; } = new();

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

    public void RemoveStoredSession(Guid sessionId)
    {
        var session = Sessions.FirstOrDefault(u => u.ConnectionId == sessionId);
        if (session != null)
        {
            Sessions.Remove(session);
        }
    }
}
