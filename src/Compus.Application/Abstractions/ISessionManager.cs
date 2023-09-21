using Compus.Domain.Client;
using Compus.Domain.Server;

namespace Compus.Application.Abstractions;
public interface ISessionManager
{
    public void CreateSession(string storageId, ExternalActiveSession activeSession);
    void SendCommand(string storageId, Guid sessionId, string command);
    TerminalOutput GetTerminalOutput(string storageId, Guid sessionId);
    bool IsConnected(string storageId, Guid sessionId);
    bool Disconnect(string storageId, Guid sessionId);
    List<ExternalActiveSession> Refresh(string storageId);
}
