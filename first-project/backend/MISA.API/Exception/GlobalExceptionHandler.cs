using Microsoft.AspNetCore.Diagnostics;
using MISA.Common.Enum;
using MISA.Common.Exception;
using MISA.Common.Model;
using MISA.Common.Resources;

namespace MISA.API.Exception;

public class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> log) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, System.Exception exception,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Handle global exception");
        var (status, message) = exception switch
        {
            AlreadyExistsException => (AppEnum.StatusCode.BadRequest, exception.Message),
            NotFoundException => (AppEnum.StatusCode.NotFound, exception.Message),
            UnauthorizedAccessException => (AppEnum.StatusCode.Unauthorized, exception.Message),
            _ => (AppEnum.StatusCode.InternalServerError, exception.Message)
        };

        // var response = new
        // {
        //     Status = status,
        //     Message = message
        // };
        var response = new ErrorResult()
        {
            DevMsg = message,
            UserMsg = ResourcesVN.Exception,
            MoreInfo = exception.ToString(),
        };
        log.LogError($"[GlobalExceptionHandler] Error happening when solving the request: {response}");

        httpContext.Response.StatusCode = (int)status;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}