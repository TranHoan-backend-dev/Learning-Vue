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
        log.LogError(exception, "[GlobalExceptionHandler] Exception occurred: {Message}", exception.Message);

        var (status, message) = exception switch
        {
            ExistingException => (ApiStatusCode.BadRequest, exception.Message),
            NotFoundException => (ApiStatusCode.NotFound, exception.Message),
            UnauthorizedAccessException => (ApiStatusCode.UnAuthorized, exception.Message),
            _ => (ApiStatusCode.InternalServerError, exception.Message)
        };

        var response = new ErrorResult()
        {
            DevMsg = message,
            UserMsg = ResourcesVN.GeneralError,
            MoreInfo = exception.ToString(),
        };
        log.LogError($"[GlobalExceptionHandler] Error happening when solving the request: {response}");

        httpContext.Response.StatusCode = (int)status;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}