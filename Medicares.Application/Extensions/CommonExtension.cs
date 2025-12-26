using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.DTO;

namespace Medicares.Application.Extensions;

internal static class CommonExtension
{
    public static string GetCompleteAddress(this Address address, string delimiter = ", ")
    {
        if (address == null)
            return string.Empty;

        List<string> parts = new List<string>
        {
            address.AddressLine, 
            address.City!,
            address.State?.Name!,
            address.PostalCode,
            address.Country
        }
        .Where(p => !string.IsNullOrWhiteSpace(p))
        .ToList();

        return string.Join(delimiter, parts);
    }

    public static AddressDto MapToAddressDto(this Address entity) => new()
    {
        Id = entity.Id,
        AddressLine = entity.AddressLine,
        PostalCode = entity.PostalCode,
        City = entity.City ?? string.Empty,
        StateId = entity.StateId,
        StateName = entity.State?.Name ?? string.Empty,
        Country = entity.Country
    };
}
