using Medicares.Domain.Base;
using Medicares.Domain.Entities.Personnel;

namespace Medicares.Domain.Entities.Auth
{
    public class Owner : BaseAuditableEntity
    {
        public Guid UserId { get; set; }

        public bool IsSubscriptionActive { get; set; }
        public DateTime? SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }

        public ApplicationUser User { get; set; } = null!;
        public ICollection<Store> Stores { get; set; } = new List<Store>();
    }
}

