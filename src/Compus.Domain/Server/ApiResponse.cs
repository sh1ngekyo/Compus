using Compus.Domain.Server.Enums;

namespace Compus.Domain.Server;

/// <summary>
/// Response from the server
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Response status
    /// </summary>
    public ResponseStatus Status { get; set; }

    /// <summary>
    /// Message for errors and warnings
    /// </summary>
    public string? ExtraMessage { get; set; }

    /// <summary>
    /// Response result
    /// </summary>
    public T? Result { get; set; }
}
