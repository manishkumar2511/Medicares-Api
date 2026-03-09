using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.Enums;

namespace Medicares.Application.Features.Subscription_Plans.Update
{
    public class UpdateSubscriptionPlanRequest
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int StoreLimit { get; set; }
        public SubscriptionPlanType Type { get; set; }
        public SubscriptionPlanStatus Status { get; set; }
    }
}
