using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.DTO;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Auth.Login;

public class LoginEndpoint(IIdentityService identityService)
    : Endpoint<LoginRequest, LoginResponse>
{
    public override void Configure()
    {
        Post(AuthGroup.AuthApiRoutes.Login);
        AllowAnonymous();
        Group<AuthGroup>();
        Summary(s =>
        {
            s.Summary = AuthGroup.AuthConsts.LoginSummary;
            s.Description = AuthGroup.AuthConsts.LoginDescription;
        });
    }

    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        LoginResult result =
            await identityService.LoginAsync(req.Email, req.Password, true, ct);

        if (!string.IsNullOrEmpty(result.Error))
        {
            AddError(result.Error);
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        if (result.RequiresMfa)
        {
            (bool success, string? error) =
                await identityService.SendMfaCodeAsync(req.Email, ct);

            if (!success)
            {
                AddError(error ?? AuthGroup.AuthMessages.FailedToSendMFA);
                await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
                return;
            }

            ApplicationUser mfaUser = result.User!;
            string mfaRole = result.Role!;
            Guid mfaRoleId = await identityService.GetRoleIdAsync(mfaUser.Id, mfaRole);

            await SendOkAsync(new LoginResponse
            {
                RequiresMfa = true,
                Message = AuthGroup.AuthMessages.MFACodeSent,
                User = new UserDto
                {
                    Id = mfaUser.Id,
                    Email = mfaUser.Email!,
                    FirstName = mfaUser.FirstName ?? string.Empty,
                    LastName = mfaUser.LastName ?? string.Empty,
                    PhoneNumber = mfaUser.PhoneNumber ?? string.Empty,
                    Role = mfaRole,
                    RoleId = mfaRoleId,
                    OwnerId = mfaUser.OwnerId,
                    IsActive = mfaUser.IsActive,
                    ProfileImageUrl = mfaUser.ProfilePictureUrl ?? string.Empty
                }
            }, ct);
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

