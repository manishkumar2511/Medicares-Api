using Medicares.Domain.Base;
using Medicares.Domain.Shared.Constant;
namespace Medicares.Domain.Entities.Common
{
    public class Address : BaseEntity
    {
        public required string AddressLine { get; set; }
        public required string PostalCode { get; set; }
        public string? City { get; set; } = string.Empty;
        public State State { get; set; } = default!;
        public required Guid StateId { get; set; }
        public string Country { get; set; } = ApplicationConsts.Country;

    }
}
