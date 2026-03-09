using Medicares.Domain.Base;
using Medicares.Domain.Shared.Enums;

namespace Medicares.Domain.Entities.Common
{
    public class SubscriptionPlan : BaseAuditableEntity
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int StoreLimit { get; set; }
        public SubscriptionPlanType Type { get; set; } = SubscriptionPlanType.Basic;
        public SubscriptionPlanStatus Status { get; set; } = SubscriptionPlanStatus.Draft;
    }
}
