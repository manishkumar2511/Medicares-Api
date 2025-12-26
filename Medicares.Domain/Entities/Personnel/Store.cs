using Medicares.Domain.Base;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Entities.Common;

namespace Medicares.Domain.Entities.Personnel
{
    public class Store : BaseAuditableEntity
    {
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
        public string LicenseNumber { get; set; } = null!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;

        public Guid AddressId { get; set; }
        public Address Address { get; set; } = default!;

        public bool IsActive { get; set; } = true;

        public Owner Owner { get; set; } = default!;
        public ICollection<StoreStaff> StoreStaffs { get; set; } = new List<StoreStaff>();


    }
}
