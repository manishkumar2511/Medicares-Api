using Medicares.Domain.Entities.Common;

namespace Medicares.Application.Features.Subscription_Plans.Create
{
    public static class CreateSubscriptionPlanMapper
    {
        public static SubscriptionPlan MapToEntity(this CreateSubscriptionPlanRequest request)
        {
            return new SubscriptionPlan
            {
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                DurationInDays = request.DurationInDays,
                StoreLimit = request.StoreLimit,
                Type = request.Type,
                Status = request.Status
            };
        }

        public static CreateSubscriptionPlanResponse MapToResponse(this SubscriptionPlan entity)
        {
            return new CreateSubscriptionPlanResponse
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                Price = entity.Price,
                DurationInDays = entity.DurationInDays,
                StoreLimit = entity.StoreLimit,
                Type = entity.Type.ToString(),
                Status = entity.Status.ToString()
            };
        }
    }
}
