using Compus.Domain.Server.Enums;
using Compus.Domain.Server;

namespace Compus.Application.Extensions;
public static class ApiResponseExtension
{
    private static ApiResponse<T> Create<T>(this ApiResponse<T> apiResponse) => new ApiResponse<T>
    {
        Status = ResponseStatus.Success
    };
}
