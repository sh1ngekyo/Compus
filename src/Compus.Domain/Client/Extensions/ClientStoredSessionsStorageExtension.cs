namespace Compus.Domain.Client.Extensions;

public static class ClientStoredSessionsStorageExtension
{
    public static void AddOrUpdateStoredSessions(this ClientStoredSessionsStorage Storage, ClientStoredSession session)
    {
        var exist = Storage.Sessions.FirstOrDefault(u => u.Id == session.Id);
        if (exist != null)
        {
            if (!session.Equals(exist))
            {
                Storage.Sessions.Remove(exist);
                Storage.Sessions.Add(session);
            }
        }
        else
        {
            Storage.Sessions.Add(session);
        }
    }

    public static void RemoveStoredSession(this ClientStoredSessionsStorage Storage, Guid sessionKey)
    {
        var session = Storage.Sessions.FirstOrDefault(u => u.Id == sessionKey);
        if (session != null)
        {
            Storage.Sessions.Remove(session);
        }
    }
}
