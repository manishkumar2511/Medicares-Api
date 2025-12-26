using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models;
using Medicares.Application.Features.Auth.Login;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.Constant;
using Medicares.Domain.Shared.DTO;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Auth.GetStarted;

public class GetStartedEndpoint(IIdentityService identityService) : Endpoint<GetStartedRequest, LoginResponse>
{
    public override void Configure()
    {
        Post(AuthGroup.AuthApiRoutes.GetStarted);
        AllowAnonymous();
        Group<AuthGroup>();
        Summary(s =>
        {
            s.Summary = AuthGroup.AuthConsts.RegisterOwnerSummary;
            s.Description = AuthGroup.AuthConsts.RegisterOwnerDescription;
        });
    }

    public override async Task HandleAsync(GetStartedRequest req, CancellationToken ct)
    {
        (Owner? owner, string? ownerError) = await identityService.CreateOwnerAsync($"{req.FirstName} {req.LastName}", req.CompanyName, ct);
        if (owner == null)
        {
            AddError(ownerError ?? "Failed to create owner");
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        (ApplicationUser? user, string? userError) = await identityService.CreateUserAsync(new UserDto
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            PhoneNumber = req.PhoneNumber,
            Role = RoleConsts.Admin,
            OwnerId = owner.Id
        }, req.Password, null, ct);
        
        if (user == null)
        {
             AddError(userError ?? "Failed to create user");
             await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
             return;
        }

        LoginResult loginResult = await identityService.LoginAsync(req.Email, req.Password, false, ct);
        if (!string.IsNullOrEmpty(loginResult.Error))
        {
             AddError(AuthGroup.AuthMessages.RegistrationLoginFailed + loginResult.Error);
             await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
             return;
        }

        IList<string> roles = await identityService.GetRolesAsync(user);
        string userRole = roles.FirstOrDefault() ?? string.Empty;
        Guid roleId = await identityService.GetRoleIdAsync(user.Id, userRole);

        await SendOkAsync(new LoginResponse
        {
            RequiresMfa = false,
            Message = AuthGroup.AuthMessages.RegistrationSuccessful, 
            Token = loginResult.AccessToken,
            RefreshToken = loginResult.RefreshToken,
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
