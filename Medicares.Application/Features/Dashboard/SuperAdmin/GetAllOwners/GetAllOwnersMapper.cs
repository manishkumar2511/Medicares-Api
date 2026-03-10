using Medicares.Domain.Entities.Auth;

namespace Medicares.Application.Features.Dashboard.SuperAdmin.GetAllOwners
{
    public static class GetAllOwnersMapper
    {
        public static List<GetAllOwnersResponse> MapToResponse(this List<Owner> owners)
        {
            return owners.Select(x =>
            {
                ApplicationUser? user = x.User;
                return new GetAllOwnersResponse
                {
                    Id = x.Id,
                    FirstName = user?.FirstName ?? string.Empty,
                    LastName = user?.LastName ?? string.Empty,
                    Email = user?.Email ?? string.Empty,
                    Phone = user?.PhoneNumber ?? string.Empty,
                    IsActive = user?.IsActive ?? false,
                    CreatedAt = x.CreatedAt,
                    LastLoginAt = user?.LastLoginAt,
                    Address = user?.Address?.AddressLine,
                    State = user?.Address?.State?.ShortName,
                    City = user?.Address?.City,
                    PostalCode = user?.Address?.PostalCode
                };
            }).ToList();
        }
    }
}
