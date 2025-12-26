using Medicares.Domain.Base;
using Medicares.Domain.Entities.Auth;
namespace Medicares.Domain.Entities.Personnel
{
    public class StoreStaff : BaseAuditableEntity
    {
        public Guid StoreId { get; set; }
        public Store Store { get; set; } = default!;

        public Guid UserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = default!;

        public decimal Salary { get; set; }

        public DateTime JoinedAt { get; set; }
        public DateTime? LeftAt { get; set; }

        public bool IsActive { get; set; }
    }
}
