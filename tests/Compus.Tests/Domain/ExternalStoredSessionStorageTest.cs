using Compus.Domain.Client;

namespace Compus.Tests.Domain;
public class ExternalStoredSessionStorageTest
{
    [Fact]
    public void RenewStoredSession_ShouldCreateNewIfNotContainsSession()
    {
        var storage = new ExternalStoredSessionStorage();
        var id = Guid.NewGuid();

        storage.RenewStoredSession(new ExternalStoredSession()
        {
            ConnectionId = id,
        });

        Assert.Single(storage.Sessions);
        Assert.Equal(id, storage.Sessions.FirstOrDefault()!.ConnectionId);
    }

    [Fact]
    public void RenewStoredSession_ShouldUpdateIfContainsSession()
    {
        var id = Guid.NewGuid();
        var updatedPort = 1;
        var session = new ExternalStoredSession()
        {
            ConnectionId = id,
        };
        var storage = new ExternalStoredSessionStorage()
        {
            Sessions = new List<ExternalStoredSession>()
            {
                session
            }
        };
        session.Port = updatedPort;


        storage.RenewStoredSession(session);

        Assert.Single(storage.Sessions);
        Assert.Equal(id, storage.Sessions.FirstOrDefault()!.ConnectionId);
        Assert.Equal(updatedPort, storage.Sessions.FirstOrDefault()!.Port);
    }
}
