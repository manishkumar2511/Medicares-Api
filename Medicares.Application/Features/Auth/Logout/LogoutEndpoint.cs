using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Extensions;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Auth.Logout;

public class LogoutEndpoint(IIdentityService identityService) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get(AuthGroup.AuthApiRoutes.Logout);
        Group<AuthGroup>();
        Summary(s =>
        {
            s.Summary = AuthGroup.AuthConsts.UserLogoutSummary;
            s.Description = AuthGroup.AuthConsts.UserLogOutDescription;
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        Guid userId = User.GetUserId();
        (bool success, string? error) = await identityService.LogoutAsync(userId, ct);

        if (!success)
        {
            AddError(error ?? AuthGroup.AuthMessages.LogOutFailed);
            await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
            return;
        }

        await SendOkAsync(new { Message = AuthGroup.AuthMessages.LogOutSuccess }, ct);
    }
}
