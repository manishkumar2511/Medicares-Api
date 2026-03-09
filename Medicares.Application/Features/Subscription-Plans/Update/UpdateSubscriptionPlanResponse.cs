namespace Medicares.Application.Features.Subscription_Plans.Update
{
    public class UpdateSubscriptionPlanResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int DurationInDays { get; set; }
        public int StoreLimit { get; set; }
        public string Type { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
