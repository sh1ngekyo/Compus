using Compus.Domain.Server.Enums;

namespace Compus.Domain.Server;

public class ApiResponse<T>
{
    public ResponseStatus Status { get; set; }
    public string? ExtraMessage { get; set; }
    public T? Result { get; set; }
}
