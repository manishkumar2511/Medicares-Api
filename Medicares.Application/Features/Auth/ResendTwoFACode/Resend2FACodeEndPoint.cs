using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Auth.ResendTwoFACode
{
    public class Resend2FACodeEndPoint(IIdentityService identityService) : Endpoint<Resend2FACodeRequest>
    {
        public override void Configure()
        {
            Post(AuthGroup.AuthApiRoutes.Resend2FACode);
            AllowAnonymous();
            Group<AuthGroup>();
            Summary(s =>
            {
                s.Summary = AuthGroup.AuthConsts.ResendMfaSummary;
                s.Description = AuthGroup.AuthConsts.ResendMfaDescription;
            });
        }

        public override async Task HandleAsync(Resend2FACodeRequest req, CancellationToken ct)
        {
            (bool success, string? error) = await identityService.SendMfaCodeAsync(req.Email, ct);

            if (!success)
            {
                AddError(error ?? AuthGroup.AuthMessages.FailedToSendMFA);
                await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
                return;
            }

            await SendOkAsync(new { Message = AuthGroup.AuthMessages.MFACodeSent }, ct);
        }
    }
}
