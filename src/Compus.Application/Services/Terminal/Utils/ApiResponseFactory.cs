using Compus.Domain.Server.Enums;
using Compus.Domain.Server;

namespace Compus.Application.Services.Terminal.Utils;

internal class ApiResponseFactory
{
    public static ApiResponse<T> Create<T>() => new ApiResponse<T>
    {
        Status = ResponseStatus.Success
    };
}
