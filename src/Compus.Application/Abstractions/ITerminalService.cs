using System.Linq.Expressions;
using Compus.Domain.Client;
using Compus.Domain.Server;

namespace Compus.Application.Abstractions;

/// <summary>
/// Terminal management service
/// </summary>
public interface ITerminalService
{
    /// <summary>
    /// Try-Catch wrapper for innier service functions
    /// </summary>
    /// <param name="action">Innier service function</param>
    /// <returns>Generic response for controller</returns>
    public ApiResponse<T> Try<T>(Expression<Func<ApiResponse<T>>> action);

    /// <summary>
    /// Connect to remote host address
    /// </summary>
    /// <param name="session">Session for connecction</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <returns>Connected session</returns>
    public ApiResponse<ExternalActiveSession> Connect(ExternalActiveSession session, string storageId);

    /// <summary>
    /// Disconnect from remote server
    /// </summary>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <returns>True if user was disconected</returns>
    public ApiResponse<bool> Disconnect(Guid sessionId, string storageId);

    /// <summary>
    /// Send command to remote terminal
    /// </summary>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <param name="command">Command to execute</param>
    /// <returns>True if the command completed successfully</returns>
    public ApiResponse<bool> ExecuteCommand(Guid sessionId, string storageId, string command);

    /// <summary>
    /// Get terminal output
    /// </summary>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <returns>Terminal output</returns>
    public ApiResponse<TerminalContent> GetView(Guid sessionId, string storageId);

    /// <summary>
    /// Get connection status
    /// </summary>
    /// <param name="sessionId">Session Id</param>
    /// <param name="storageId">Id for current session's storage</param>
    /// <returns>True if connected to terminal, otherwise false</returns>
    public ApiResponse<bool> IsConnected(Guid sessionId, string storageId);

    /// <summary>
    /// Get list with all connected session for current user
    /// </summary>
    /// <param name="storageId">Id for current session's storage</param>
    /// <returns>All connected sessions</returns>
    public ApiResponse<List<ExternalActiveSession>> GetAllConnectedSessions(string storageId);
}
