using Compus.Application.Abstractions;
using Compus.Domain.Client;
using Compus.Domain.Server;
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

        public ApiResponse<bool> ExecuteCommand(Guid uniqueId, string command)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            return _terminalService.ExecuteCommand(uniqueId, storageId!, command);
        }

        public ApiResponse<TerminalContent> GetView(Guid uniqueId)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            return _terminalService.GetView(uniqueId, storageId!);
        }

        public ApiResponse<bool> IsConnected(Guid uniqueId)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            return _terminalService.IsConnected(uniqueId, storageId!);
        }

        public ApiResponse<bool> Disconnect(Guid uniqueId)
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            return _terminalService.Disconnect(uniqueId, storageId!);
        }

        public ApiResponse<List<ExternalActiveSession>> GetAllConnectedSessions()
        {
            var storageId = HttpContext.Session.GetString(Constants.ClientSessionIdName);
            return _terminalService.GetAllConnectedSessions(storageId!);
        }
    }
}
