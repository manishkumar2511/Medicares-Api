using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Application.Features.Auth.Login;

public class LoginEndpoint(IIdentityService identityService) : Endpoint<LoginRequest, LoginResponse>
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
        LoginResult result = await identityService.LoginAsync(req.Email, req.Password, true, ct);

        if (!string.IsNullOrEmpty(result.Error))
        {
            AddError(result.Error);
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);     
            return;
        }

        if (result.RequiresTwoFactor)
        {
            (bool success, string? error) = await identityService.SendMfaCodeAsync(req.Email, ct);
            if (!success)
            {
                AddError(error ?? AuthGroup.AuthMessages.FailedToSendMFA);
                await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
                return;
            }

            await SendOkAsync(new LoginResponse
            {
                RequiresMfa = true,
                Message = AuthGroup.AuthMessages.MFACodeSent
            }, ct);
            return;
        }
        
        ApplicationUser? user = await identityService.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);
        if (user == null) 
        {
             AddError("User not found after login");
             await SendErrorsAsync(StatusCodes.Status500InternalServerError, ct);
             return;
        }

        IList<string> roles = await identityService.GetRolesAsync(user);
        string userRole = roles.FirstOrDefault() ?? string.Empty;
        Guid roleId = await identityService.GetRoleIdAsync(user.Id, userRole);

        await SendOkAsync(new LoginResponse
        {
            RequiresMfa = false,
            Message = AuthGroup.AuthMessages.LoginSuccessful,
            Token = result.AccessToken,
            RefreshToken = result.RefreshToken,
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
