using Compus.Domain.Client;
using Compus.Domain.Server;
using Renci.SshNet;

namespace Compus.Application.Abstractions;

public interface ITerminalService
{
    public ApiResponse<ExternalActiveSession> Connect(ExternalActiveSession activeSessionModel, string storageId);
    public ApiResponse<bool> Disconnect(Guid sessionId, string storageId);
    public ApiResponse<bool> ExecuteCommand(Guid sessionId, string storageId, string command);
    public ApiResponse<TerminalOutput> GetView(Guid sessionId, string storageId);
    public ApiResponse<bool> IsConnected(Guid sessionId, string storageId);
    public ApiResponse<List<ExternalActiveSession>> GetAllConnectedSessions(string storageId);
}
