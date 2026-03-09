using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.Enums;

namespace Medicares.Application.Features.Subscription_Plans.Update
{
    public static class UpdateSubscriptionPlanMapper
    {
        public static void UpdateEntity(this UpdateSubscriptionPlanRequest request, SubscriptionPlan entity)
        {
            entity.Name = request.Name;
            entity.Description = request.Description;
            entity.Price = request.Price;
            entity.DurationInDays = request.DurationInDays;
            entity.StoreLimit = request.StoreLimit;
            entity.Type = request.Type;
            entity.Status = request.Status;
        }

        public static UpdateSubscriptionPlanResponse MapToResponse(this SubscriptionPlan entity)
        {
            return new UpdateSubscriptionPlanResponse
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
