using FastEndpoints;
using Medicares.Application.Contracts.Extensions;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Models;
using Medicares.Application.Features.Auth.Login;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.Constant;
using Medicares.Domain.Shared.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Application.Features.Auth.GetStarted;

public class GetStartedEndpoint(IIdentityService identityService, IUnitOfWork unitOfWork) : Endpoint<GetStartedRequest, object>
{
    public override void Configure()
    {
        Post(AuthGroup.AuthApiRoutes.GetStarted);
        AllowAnonymous();
        Group<AuthGroup>();
        Summary(s =>
        {
            s.Summary = AuthGroup.AuthConsts.OwnerRegistrationSummary;
            s.Description = AuthGroup.AuthConsts.OwnerRegistrationDescription;
        });
    }

    public override async Task HandleAsync(GetStartedRequest req, CancellationToken ct)
    {
        bool userExists = await identityService.Users.AnyAsync(u => u.Email == req.Email, ct);
        if (userExists)
        {
            AddError(AuthGroup.AuthMessages.OwnerEmailAlreadyExists);
            await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            return;
        }

        await unitOfWork.ExecuteTransactionAsync(async () =>
        {
            Address address = req.MapToAddress();
            Owner owner = req.MapToOwner();

            await unitOfWork.Repository<Address>().AddAsync(address, ct);
            
            (Owner? createdOwner, string? ownerError) = await identityService.CreateOwnerAsync(owner, ct);
            if (createdOwner == null) ThrowError(ownerError ?? AuthGroup.AuthMessages.RegistrationFailed);

            UserDto userDto = req.MapToUserDto(createdOwner.Id);
            (ApplicationUser? user, string? userError) = await identityService.CreateUserAsync(userDto, req.Password, address.Id, ct);
            
            if (user == null) ThrowError(userError ?? AuthGroup.AuthMessages.RegistrationFailed);
        }, ct);

        await SendOkAsync(new
        {
            Message = AuthGroup.AuthMessages.RegistrationSuccessful
        }, ct);
    }
}
