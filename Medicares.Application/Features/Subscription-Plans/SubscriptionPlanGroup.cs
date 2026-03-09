using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Subscription_Plans
{
    public class SubscriptionPlanGroup : Group
    {
        public SubscriptionPlanGroup()
        {
            base.Configure(
                "subscription-plan",
                ep =>
                {
                    ep.Description(
                        x => x.Produces(401)
                              .WithTags("Subscription-Plan")); // Match route-auto-generated tag exactly
                });
        }

        public static class ApiRoutes
        {
            public const string Create = "create";
        }

        public static class SubscriptionPlanConsts
        {
            public const string CreateSummary = "Create Subscription Plan";
            public const string CreateDescription = "Endpoint to create a new subscription plan with name, price, and duration.";
        }

        public static class SubscriptionPlanMessages
        {
            public const string CreateSuccess = "Subscription plan created successfully";
            public const string CreateFailed = "Failed to create subscription plan";
        }
    }
}
