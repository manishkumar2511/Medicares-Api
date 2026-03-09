using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Wrappers;
using Medicares.Domain.Entities.Common;
using Medicares.Application.Features.Subscription_Plans;
using Medicares.Domain.Shared.Constant;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Application.Features.Subscription_Plans.GetAll
{
    public class GetAllSubscriptionPlansEndpoint(IUnitOfWork unitOfWork) : EndpointWithoutRequest<Result<List<GetAllSubscriptionPlansResponse>>>
    {
        public override void Configure()
        {
            Get(SubscriptionPlanGroup.ApiRoutes.GetAll);
            Group<SubscriptionPlanGroup>();
            AllowAnonymous();

            Summary(s =>
            {
                s.Summary = "Get All Subscription Plans";
                s.Description = "Returns a list of all available subscription plans.";
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            GetAllSubscriptionPlansSpecification spec = new GetAllSubscriptionPlansSpecification();
            List<SubscriptionPlan> plans = await unitOfWork.Repository<SubscriptionPlan>()
                .GetWithSpecificationAsync(spec, ct);

            List<GetAllSubscriptionPlansResponse> response = plans.MapToResponse();

            await SendOkAsync(await Result<List<GetAllSubscriptionPlansResponse>>.SuccessAsync(response), ct);
        }
    }
}
