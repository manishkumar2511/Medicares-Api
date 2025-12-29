using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models;
using Medicares.Application.Features.Auth.Login;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.DTO;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Auth.Verify2FACode;

public class Verify2FACodeEndPoint(IIdentityService identityService) 
    : Endpoint<Verify2FACodeRequest, LoginResponse>
{
    public override void Configure()
    {
        Post(AuthGroup.AuthApiRoutes.Verify2FACode);
        AllowAnonymous();
        Group<AuthGroup>();
        Summary(s =>
        {
            s.Summary = "Verify 2FA Code";
            s.Description = "Verifies the MFA code and returns auth token";
        });
    }

    public override async Task HandleAsync(Verify2FACodeRequest req, CancellationToken ct)
    {
        LoginResult result = await identityService.Verify2FACodeAsync(req.Email, req.Code, ct);

        if (!string.IsNullOrEmpty(result.Error))
        {
            AddError(result.Error);
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        ApplicationUser user = result.User!;
        string userRole = result.Role!;
        Guid roleId = await identityService.GetRoleIdAsync(user.Id, userRole);

        await SendOkAsync(new LoginResponse
        {
            RequiresMfa = false,
            Message = AuthGroup.AuthMessages.LoginSuccessful,
            Token = result.Token!.AccessToken,
            RefreshToken = result.RefreshToken!.Token,
            User = new UserDto
            {
                Id = user.Id,
                Email = user.Email!,
                FirstName = user.FirstName ?? string.Empty,
                LastName = user.LastName ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Role = userRole,
                RoleId = roleId,
                OwnerId = user.OwnerId,
                IsActive = user.IsActive,
                ProfileImageUrl = user.ProfilePictureUrl ?? string.Empty
            }
        }, ct);
    }
}
