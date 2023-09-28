using Compus.Application.Services.Terminal.Utils;
using Compus.Domain.Server;

namespace Compus.Application.Extensions;

public static class InternalSessionStorageExtension
{
    /// <summary>
    /// Get expired session from storage based on server configuration
    /// </summary>
    /// <param name="storage">Session storage</param>
    /// <param name="config">Server config</param>
    /// <returns>All expired session from storage</returns>
    public static IEnumerable<KeyValuePair<Guid, InternalActiveSession>> GetExpiredSessions(this InternalSessionStorage storage, ServerConfig config)
    {
        var validateTime = (DateTime sessionTime) 
            => sessionTime < DateTime.Now.AddMinutes(-config.MaxIdleMinutes);

        return storage.Sessions.Where(
                u => validateTime(u.Value.LastActiveSessionDate) ||
                !u.Value.SshClient!.IsConnected);
    }
}
