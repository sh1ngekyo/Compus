using Compus.Application.Services.Terminal.Utils;
using Compus.Domain.Server;

namespace Compus.Application.Extensions;

public static class InternalSessionStorageExtension
{
    public static IEnumerable<KeyValuePair<Guid, InternalActiveSession>> GetExpiredSessions(this InternalSessionStorage storage, ServerConfig config)
    {
        var validateTime = (DateTime sessionTime) 
            => sessionTime < DateTime.Now.AddMinutes(-config.MaxIdleMinutes);

        return storage.Sessions.Where(
                u => validateTime(u.Value.LastActiveSessionDate) ||
                !u.Value.SshClient!.IsConnected);
    }
}
