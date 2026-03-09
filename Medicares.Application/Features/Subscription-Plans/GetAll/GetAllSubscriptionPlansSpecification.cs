using Medicares.Application.Contracts.Specifications;
using Medicares.Domain.Entities.Common;

namespace Medicares.Application.Features.Subscription_Plans.GetAll
{
    public class GetAllSubscriptionPlansSpecification : BaseSpecification<SubscriptionPlan>
    {
        public GetAllSubscriptionPlansSpecification()
        {
            OrderBy = "Name";
        }
    }
}
