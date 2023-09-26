using Compus.Domain.Server.Enums;

namespace Compus.Domain.Server;

public class ApiResponse<T>
{
    public ResponseStatus StatusResult { get; set; }
    public string? ExtraMessage { get; set; }
    public T? Response { get; set; }
}
