using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.DTO;
using Medicares.Domain.Shared.Constant;
using Medicares.Domain.Entities.Common;

namespace Medicares.Application.Features.Auth.GetStarted;

public static class GetStartedMapper
{
    public static Owner MapToOwner(this GetStartedRequest request, Guid userId)
    {
        return new Owner
        {
            UserId = userId,
            IsSubscriptionActive = false,
            SubscriptionStartDate = null,
            SubscriptionEndDate = null
        };
    }

    public static Address MapToAddress(this GetStartedRequest request)
    {
        return new Address
        {
            AddressLine = request.AddressLine,
            PostalCode = request.PostalCode,
            City = request.City,
            StateId = request.StateId,
            Country = ApplicationConsts.Country
        };
    }

    public static UserDto MapToUserDto(this GetStartedRequest request, Guid? ownerId = null)
    {
        return new UserDto
        {
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.PhoneNumber,
            IsActive = true,
            Role = RoleConsts.Admin,
            OwnerId = ownerId
        };
    }
}
