using Compus.Application.Abstractions;
using Compus.Domain.Client;
using Compus.Domain.Server;
using Compus.Domain.Server.Enums;

namespace Compus.Application.Services.Terminal;

public class TerminalService : ITerminalService
{
    private readonly ConnectionManager? _connectionManager;

    public TerminalService(ConnectionManager connectionManager)
        => _connectionManager = connectionManager;

    private ApiResponse<T> CreateResponse<T>() => new ApiResponse<T>
    {
        StatusResult = ResponseStatus.Success
    };

    public ApiResponse<ExternalActiveSession> Connect(ExternalActiveSession activeSessionModel, string storageId)
    {
        var response = CreateResponse<ExternalActiveSession>();

        try
        {
            if (activeSessionModel.StoredSession != null)
            {
                _connectionManager!.AddConnection(storageId, activeSessionModel);

                response.Response = new ExternalActiveSession
                {
                    StartSessionDate = DateTime.Now,
                    Status = "Connected Successful",
                    StoredSession = activeSessionModel.StoredSession,
                    ConnectionId = activeSessionModel.ConnectionId
                };
            }
        }
        catch (Exception ex)
        {
            response.StatusResult = ResponseStatus.Exception;
            response.ExtraMessage = ex.Message;
        }
        return response;
    }

    public ApiResponse<bool> Disconnect(Guid sessionId, string storageId)
    {
        var response = CreateResponse<bool>();

        try
        {
            if (string.IsNullOrEmpty(storageId))
            {
                response.StatusResult = ResponseStatus.Fail;
                response.ExtraMessage = "No active sessions";
            }
            else
            {
                response.Response = _connectionManager!.Disconnect(storageId, sessionId);
            }
        }
        catch (Exception ex)
        {
            response.StatusResult = ResponseStatus.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<bool> ExecuteCommand(Guid sessionId, string storageId, string command)
    {
        var response = CreateResponse<bool>();

        try
        {
            if (string.IsNullOrEmpty(storageId))
            {
                response.StatusResult = ResponseStatus.Fail;
                response.ExtraMessage = "No active sessions";
            }
            else
            {
                _connectionManager!.ExecuteCommand(storageId, sessionId, command);
                response.Response = true;
            }
        }
        catch (Exception ex)
        {
            response.StatusResult = ResponseStatus.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<List<ExternalActiveSession>> GetAllConnectedSessions(string storageId)
    {
        var response = CreateResponse<List<ExternalActiveSession>>();

        try
        {
            if (string.IsNullOrEmpty(storageId))
            {
                response.Response = new List<ExternalActiveSession>();
            }
            else
            {
                response.Response = _connectionManager!.FlushStorage(storageId);
            }
        }
        catch (Exception ex)
        {
            response.StatusResult = ResponseStatus.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<TerminalContent> GetView(Guid sessionId, string storageId)
    {
        var response = CreateResponse<TerminalContent>();

        try
        {
            if (string.IsNullOrEmpty(storageId))
            {
                response.StatusResult = ResponseStatus.Fail;
                response.ExtraMessage = "No active sessions";
            }
            else
            {
                response.Response = _connectionManager!.GetTerminalOutput(storageId, sessionId);
            }
        }
        catch (Exception ex)
        {
            response.StatusResult = ResponseStatus.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<bool> IsConnected(Guid sessionId, string storageId)
    {
        var response = CreateResponse<bool>();

        try
        {
            if (string.IsNullOrEmpty(storageId))
            {
                response.StatusResult = ResponseStatus.Fail;
                response.ExtraMessage = "No active sessions";
            }
            else
            {
                response.Response = _connectionManager!.IsConnected(storageId, sessionId);
            }
        }
        catch (Exception ex)
        {
            response.StatusResult = ResponseStatus.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }
}
