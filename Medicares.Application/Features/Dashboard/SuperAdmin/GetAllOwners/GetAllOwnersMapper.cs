using Medicares.Domain.Entities.Auth;

namespace Medicares.Application.Features.Dashboard.SuperAdmin.GetAllOwners
{
    public static class GetAllOwnersMapper
    {
        public static List<GetAllOwnersResponse> MapToResponse(this List<Owner> owners)
        {
            return owners.Select(x =>
            {
                ApplicationUser? user = x.ApplicationUser.FirstOrDefault();
                return new GetAllOwnersResponse
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Phone = x.Phone,
                    IsActive = x.IsActive,
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
