using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Wrappers;
using Medicares.Domain.Entities.Common;
using Medicares.Domain.Shared.Constant;

namespace Medicares.Application.Features.Subscription_Plans.Delete
{
    public class DeleteSubscriptionPlanEndpoint(IUnitOfWork unitOfWork) : Endpoint<DeleteSubscriptionPlanRequest, Result<Guid>>
    {
        public override void Configure()
        {
            Delete(SubscriptionPlanGroup.ApiRoutes.Delete);
            Group<SubscriptionPlanGroup>();
            Roles(RoleConsts.SuperAdmin);
            Summary(s =>
            {
                s.Summary = SubscriptionPlanGroup.SubscriptionPlanConsts.DeleteSummary;
                s.Description = SubscriptionPlanGroup.SubscriptionPlanConsts.DeleteDescription;
            });
        }

        public override async Task HandleAsync(DeleteSubscriptionPlanRequest req, CancellationToken ct)
        {
            SubscriptionPlan? subscriptionPlan = await unitOfWork.Repository<SubscriptionPlan>().GetByIdAsync(req.Id, ct);

            if (subscriptionPlan == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            await unitOfWork.Repository<SubscriptionPlan>().DeleteAsync(subscriptionPlan, ct);
            await unitOfWork.SaveAsync(ct);

            await SendOkAsync(await Result<Guid>.SuccessAsync(subscriptionPlan.Id, SubscriptionPlanGroup.SubscriptionPlanMessages.DeleteSuccess), ct);
        }
    }
}
