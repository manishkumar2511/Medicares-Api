using FastEndpoints;
using Medicares.Application.Contracts.Extensions;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Notification;
using Medicares.Application.Contracts.Wrappers;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Application.Features.Auth.GetStarted;

public class GetStartedEndpoint(IIdentityService identityService, IUnitOfWork unitOfWork, IEmailService emailService) : Endpoint<GetStartedRequest, Result<bool>>
{
    public override void Configure()
    {
        Post(AuthGroup.AuthApiRoutes.GetStarted);
        AllowAnonymous();
        AllowFileUploads();
        Group<AuthGroup>();
        Summary(s =>
        {
            s.Summary = AuthGroup.AuthConsts.OwnerRegistrationSummary;
            s.Description = AuthGroup.AuthConsts.OwnerRegistrationDescription;
        });
    }

    public override async Task HandleAsync(GetStartedRequest req, CancellationToken ct)
    {
        ApplicationUser? existingUser = await identityService.Users.FirstOrDefaultAsync(u => u.Email == req.Email, ct);
        if (existingUser is not null)
        {
            await SendOkAsync(await Result<bool>.FailAsync(AuthGroup.AuthMessages.OwnerEmailAlreadyExists), ct);
            return;
        }

        try
        {
            await unitOfWork.ExecuteTransactionAsync(async () =>
            {
                Address address = req.MapToAddress();
                Owner owner = req.MapToOwner();

                await unitOfWork.Repository<Address>().AddAsync(address, ct);

                (Owner? createdOwner, string? ownerError) = await identityService.CreateOwnerAsync(owner, ct);
                if (createdOwner == null) throw new Exception(ownerError ?? AuthGroup.AuthMessages.RegistrationFailed);

                UserDto userDto = req.MapToUserDto(createdOwner.Id);
                (ApplicationUser? user, string? userError) = await identityService.CreateUserAsync(userDto, req.Password, address.Id, ct);

                if (user == null) throw new Exception(userError ?? AuthGroup.AuthMessages.RegistrationFailed);
            }, ct);

            await emailService.SendWelcomeEmailAsync(req.Email, $"{req.FirstName} {req.LastName}", ct);

            await SendOkAsync(await Result<bool>.SuccessAsync(true, AuthGroup.AuthMessages.RegistrationSuccessful), ct);
        }
        catch (Exception ex)
        {
            await SendOkAsync(await Result<bool>.FailAsync(ex.Message), ct);
        }
    }
}
