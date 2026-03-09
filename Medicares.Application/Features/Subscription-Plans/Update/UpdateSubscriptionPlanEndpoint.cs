using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Wrappers;
using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.Constant;

namespace Medicares.Application.Features.Subscription_Plans.Update
{
    public class UpdateSubscriptionPlanEndpoint(IUnitOfWork unitOfWork) : Endpoint<UpdateSubscriptionPlanRequest, Result<UpdateSubscriptionPlanResponse>>
    {
        public override void Configure()
        {
            Put(SubscriptionPlanGroup.ApiRoutes.Update);
            Group<SubscriptionPlanGroup>();
            Roles(RoleConsts.SuperAdmin);
            Summary(s =>
            {
                s.Summary = SubscriptionPlanGroup.SubscriptionPlanConsts.UpdateSummary;
                s.Description = SubscriptionPlanGroup.SubscriptionPlanConsts.UpdateDescription;
            });
        }

        public override async Task HandleAsync(UpdateSubscriptionPlanRequest req, CancellationToken ct)
        {
            SubscriptionPlan? plan = await unitOfWork.Repository<SubscriptionPlan>().GetByIdAsync(req.Id, ct);

            if (plan == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            req.UpdateEntity(plan);
            await unitOfWork.Repository<SubscriptionPlan>().UpdateAsync(plan);
            await unitOfWork.SaveAsync(ct);

            await SendOkAsync(await Result<UpdateSubscriptionPlanResponse>.SuccessAsync(plan.MapToResponse(), SubscriptionPlanGroup.SubscriptionPlanMessages.UpdateSuccess), ct);
        }
    }
}
