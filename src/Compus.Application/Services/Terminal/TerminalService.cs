using Compus.Application.Abstractions;
using Compus.Application.Utils.Terminal;
using Compus.Domain.Client;
using Compus.Domain.Server;
using Compus.Domain.Server.Enums;

namespace Compus.Application.Services.Terminal;

public class TerminalService : ITerminalService
{
    private readonly ISessionManager? _sessionManager;

    public TerminalService(ISessionManager sessionManager)
        => _sessionManager = sessionManager;

    public ApiResponse<ExternalActiveSession> Connect(ExternalActiveSession activeSessionModel, string storageId)
    {
        var response = new ApiResponse<ExternalActiveSession>
        {
            StausResult = StatusResponse.Successful
        };

        try
        {
            _sessionManager!.CreateSession(storageId, activeSessionModel);
            response.Response = new ExternalActiveSession
            {
                StartSessionDate = DateTime.Now,
                Status = "Connected Successful",
                StoredSession = activeSessionModel.StoredSession,
                Id = activeSessionModel.Id
            };
        }
        catch (Exception ex)
        {
            response.StausResult = StatusResponse.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<bool> Disconnect(Guid sessionId, string storageId)
    {
        var response = new ApiResponse<bool>
        {
            StausResult = StatusResponse.Successful
        };

        try
        {
            response.Response = _sessionManager!.Disconnect(storageId, sessionId);
        }
        catch (Exception ex)
        {
            response.StausResult = StatusResponse.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<bool> ExecuteCommand(Guid sessionId, string storageId, string command)
    {
        var response = new ApiResponse<bool>
        {
            StausResult = StatusResponse.Successful
        };

        try
        {
            _sessionManager!.SendCommand(storageId, sessionId, command);
            response.Response = true;
        }
        catch (Exception ex)
        {
            response.StausResult = StatusResponse.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<List<ExternalActiveSession>> GetAllConnectedSessions(string storageId)
    {
        var response = new ApiResponse<List<ExternalActiveSession>>
        {
            StausResult = StatusResponse.Successful
        };

        try
        {
            response.Response = _sessionManager!.Refresh(storageId);
        }
        catch (Exception ex)
        {
            response.StausResult = StatusResponse.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<TerminalOutput> GetView(Guid sessionId, string storageId)
    {
        var response = new ApiResponse<TerminalOutput>
        {
            StausResult = StatusResponse.Successful
        };

        try
        {
            response.Response = _sessionManager!.GetTerminalOutput(storageId, sessionId);
        }
        catch (Exception ex)
        {
            response.StausResult = StatusResponse.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }

    public ApiResponse<bool> IsConnected(Guid sessionId, string storageId)
    {
        var response = new ApiResponse<bool>
        {
            StausResult = StatusResponse.Successful
        };

        try
        {
            response.Response = _sessionManager!.IsConnected(storageId, sessionId);
        }
        catch (Exception ex)
        {
            response.StausResult = StatusResponse.Exception;
            response.ExtraMessage = ex.Message;
        }

        return response;
    }
}
