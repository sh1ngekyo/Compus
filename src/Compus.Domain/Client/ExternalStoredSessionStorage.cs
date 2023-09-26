namespace Compus.Domain.Client;

public class ExternalStoredSessionStorage
{
    public void RenewStoredSession(ExternalStoredSession session)
    {
        var exist = Sessions.FirstOrDefault(u => u.ConnectionId == session.ConnectionId);
        if (exist != null)
        {
            if (!session.Equals(exist))
            {
                Sessions.Remove(exist);
                Sessions.Add(session);
            }
        }
        else
        {
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

    public List<ExternalStoredSession> Sessions { get; set; } = new();
}
