using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Wrappers;
using Medicares.Domain.Entities.Common;
using Medicares.Application.Features.Subscription_Plans;
using Medicares.Domain.Shared.Constant;

namespace Medicares.Application.Features.Subscription_Plans.Create
{
    public class CreateSubscriptionPlanEndpoint(IUnitOfWork unitOfWork) : Endpoint<CreateSubscriptionPlanRequest, Result<CreateSubscriptionPlanResponse>>
    {
        public override void Configure()
        {
            Post(SubscriptionPlanGroup.ApiRoutes.Create);
            Group<SubscriptionPlanGroup>();
            Roles(RoleConsts.SuperAdmin);
            Summary(s =>
            {
                s.Summary = SubscriptionPlanGroup.SubscriptionPlanConsts.CreateSummary;
                s.Description = SubscriptionPlanGroup.SubscriptionPlanConsts.CreateDescription;
            });
        }

        public override async Task HandleAsync(CreateSubscriptionPlanRequest req, CancellationToken ct)
        {
            SubscriptionPlan plan = req.MapToEntity();
            await unitOfWork.Repository<SubscriptionPlan>().AddAsync(plan, ct);
            await unitOfWork.SaveAsync(ct);

            await SendOkAsync(await Result<CreateSubscriptionPlanResponse>.SuccessAsync(plan.MapToResponse(), SubscriptionPlanGroup.SubscriptionPlanMessages.CreateSuccess), ct);
        }
    }
}
