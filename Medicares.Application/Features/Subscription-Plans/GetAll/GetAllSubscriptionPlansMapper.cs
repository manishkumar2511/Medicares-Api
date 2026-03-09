using Medicares.Domain.Entities.Common;

namespace Medicares.Application.Features.Subscription_Plans.GetAll
{
    public static class GetAllSubscriptionPlansMapper
    {
        public static GetAllSubscriptionPlansResponse MapToResponse(this SubscriptionPlan entity)
        {
            return new GetAllSubscriptionPlansResponse
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

        public static List<GetAllSubscriptionPlansResponse> MapToResponse(this List<SubscriptionPlan> entities)
        {
            return entities.Select(x => x.MapToResponse()).ToList();
        }
    }
}
