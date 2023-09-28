using System.Linq.Expressions;
using Compus.Domain.Client;
using Compus.Domain.Server;

namespace Compus.Application.Abstractions;

public interface ITerminalService
{
    public ApiResponse<T> Try<T>(Expression<Func<ApiResponse<T>>> action);
    public ApiResponse<ExternalActiveSession> Connect(ExternalActiveSession activeSessionModel, string storageId);
    public ApiResponse<bool> Disconnect(Guid sessionId, string storageId);
    public ApiResponse<bool> ExecuteCommand(Guid sessionId, string storageId, string command);
    public ApiResponse<TerminalContent> GetView(Guid sessionId, string storageId);
    public ApiResponse<bool> IsConnected(Guid sessionId, string storageId);
    public ApiResponse<List<ExternalActiveSession>> GetAllConnectedSessions(string storageId);
}
