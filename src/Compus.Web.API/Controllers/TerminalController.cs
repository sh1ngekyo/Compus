using Compus.Application.Abstractions;
using Compus.Domain.Client;
using Compus.Domain.Server;
using Compus.Domain.Server.Enums;
using Compus.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Compus.Web.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class TerminalController : ControllerBase
    {
        public readonly ITerminalService _terminalService;

        public TerminalController(ITerminalService terminalService)
        {
            _terminalService = terminalService;
        }

        [HttpPost]
        public ApiResponse<ExternalActiveSession> Connect(ExternalActiveSession activeSessionModel)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            if (string.IsNullOrEmpty(storageId))
            {
                storageId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString(Constants.ClientSessionIdName, storageId);
            }
            return _terminalService.Connect(activeSessionModel, storageId);
        }

        public ApiResponse<bool> ExecuteCommand(Guid sessionId, string command)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            var response = new ApiResponse<bool> 
            { 
                StausResult = StatusResponse.Successful
            };
            if (string.IsNullOrEmpty(storageId))
            {
                response.StausResult = StatusResponse.Failed;
                response.ExtraMessage = "No active sessions";
                return response;
            }
            return _terminalService.ExecuteCommand(sessionId, storageId, command);
        }

        public ApiResponse<TerminalOutput> GetTerminalView(Guid sessionId)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName); 
            var response = new ApiResponse<TerminalOutput>
            {
                StausResult = StatusResponse.Successful
            };
            if (string.IsNullOrEmpty(storageId))
            {
                response.StausResult = StatusResponse.Failed;
                response.ExtraMessage = "No active sessions";
                return response;
            }
            return _terminalService.GetView(sessionId, storageId);
        }

        public ApiResponse<bool> IsConnected(Guid sessionId)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            var response = new ApiResponse<bool>
            {
                StausResult = StatusResponse.Successful
            };
            if (string.IsNullOrEmpty(storageId))
            {
                response.StausResult = StatusResponse.Failed;
                response.ExtraMessage = "No active sessions";
                return response;
            }
            return _terminalService.IsConnected(sessionId, storageId);
        }

        public ApiResponse<bool> Disconnect(Guid sessionId)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            var response = new ApiResponse<bool>
            {
                StausResult = StatusResponse.Successful
            }; 
            if (string.IsNullOrEmpty(storageId))
            {
                response.StausResult = StatusResponse.Failed;
                response.ExtraMessage = "No active sessions";
                return response;
            }
            return _terminalService.Disconnect(sessionId, storageId);
        }

        public ApiResponse<List<ExternalActiveSession>> ConnectedSessions()
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            var response = new ApiResponse<List<ExternalActiveSession>> 
            { 
                StausResult = StatusResponse.Successful
            };
            if (string.IsNullOrEmpty(storageId))
            {
                response.Response = new List<ExternalActiveSession>();
                return response;
            }
            return _terminalService.GetAllConnectedSessions(storageId);
        }
    }
}
