using Medicares.Application.Contracts.Wrappers;
using Medicares.Domain.Shared.Constant;
using Microsoft.AspNetCore.Diagnostics;
using FluentValidation;

namespace Medicares.Api;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly Dictionary<Type, Func<HttpContext, Exception, Task>> _exceptionHandlers;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;

        // Register known exception types and handlers.
        _exceptionHandlers = new Dictionary<Type, Func<HttpContext, Exception, Task>>()
        {
            { typeof(ValidationException), HandleValidationException },
            { typeof(UnauthorizedAccessException), HandleUnauthorizedAccessException },
            // { typeof(ForbiddenAccessException), HandleForbiddenAccessException }, // Define custom exception if needed
            { typeof(ArgumentNullException), HandleArgumentNullException },
            // { typeof(NotFoundException), HandleNotFoundException } // Define custom exception if needed
        };
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Type exceptionType = exception.GetType();

        if (_exceptionHandlers.TryGetValue(exceptionType, out Func<HttpContext, Exception, Task>? handler))
        {
            // ApplicationConsts.ErrorConsts.HandleErrorType ideally
            _logger.LogWarning(exception, "Error Type: {ErrorType}", exceptionType.Name);
            await handler(httpContext, exception);
            return true;
        }

        _logger.LogError(exception, "Unhandled Exception: {Message}", exception.Message);

        httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        await httpContext.Response.WriteAsJsonAsync(
            await Result.FailAsync(new List<string> { "Unhandled Exception", exception.Message }),
            cancellationToken);

        return true; 
    }

    private async static Task HandleValidationException(HttpContext httpContext, Exception ex)
    {
        ValidationException? exception = (ValidationException)ex;
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        List<string> errors = exception.Errors.Select(e => e.ErrorMessage).ToList();

        await httpContext.Response.WriteAsJsonAsync(await Result.FailAsync(errors));
    }

    private async static Task HandleUnauthorizedAccessException(HttpContext httpContext, Exception ex)
    {
        _ = ex;
        httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;

        await httpContext.Response.WriteAsJsonAsync(await Result.FailAsync(ex.Message));
    }

    private async static Task HandleArgumentNullException(HttpContext httpContext, Exception ex)
    {
        httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;

        await httpContext.Response.WriteAsJsonAsync(await Result.FailAsync(ex.Message));
    }
}
