using Medicares.Application.Contracts.Wrappers;
using FluentValidation.Results;

namespace Medicares.Api.Middlewares;

public static class FastEndpointsErrorHandler
{
    public static object BuildErrorResponse(
        List<ValidationFailure> failures,
        HttpContext ctx,
        int statusCode)
    {
        List<string> messages = failures.Select(x => x.ErrorMessage).ToList();

        ctx.Response.StatusCode = statusCode;
        return Result.Fail(messages);
    }
}
