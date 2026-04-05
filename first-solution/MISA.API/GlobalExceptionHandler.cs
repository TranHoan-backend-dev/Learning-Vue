using BL.Dto.Response;
using DL.Exception;
using Microsoft.AspNetCore.Diagnostics;

namespace first_solution;

public class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> log
) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception,
        CancellationToken cancellationToken)
    {
        log.Log(LogLevel.Information, "Handle global exception");
        var (status, message) = exception switch
        {
            NotFoundException => (StatusCodes.Status400BadRequest, exception.Message),
            AlreadyExistsException => (StatusCodes.Status400BadRequest, exception.Message),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, exception.Message),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, exception.Message),
            _ => (StatusCodes.Status500InternalServerError, exception.Message)
        };

        var response = new ApiResponse
        {
            Status = status,
            Message = message
        };

        httpContext.Response.StatusCode = status;
        httpContext.Response.ContentType = "application.json";

        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);
        return true;
    }
}