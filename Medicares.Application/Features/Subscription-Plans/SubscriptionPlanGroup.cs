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
            public const string GetAll = "get-all";
            public const string Delete = "delete/{Id}";
            public const string Update = "update/{Id}";
        }

        public static class SubscriptionPlanConsts
        {
            public const string CreateSummary = "Create Subscription Plan";
            public const string CreateDescription = "Endpoint to create a new subscription plan with name, price, and duration.";
            public const string DeleteSummary = "Delete Subscription Plan";
            public const string DeleteDescription = "Endpoint to delete an existing subscription plan by its unique identifier.";
            public const string UpdateSummary = "Update Subscription Plan";
            public const string UpdateDescription = "Endpoint to update an existing subscription plan's details.";
        }

        public static class SubscriptionPlanMessages
        {
            public const string CreateSuccess = "Subscription plan created successfully";
            public const string CreateFailed = "Failed to create subscription plan";
            public const string DeleteSuccess = "Subscription plan deleted successfully";
            public const string DeleteFailed = "Failed to delete subscription plan";
            public const string UpdateSuccess = "Subscription plan updated successfully";
            public const string UpdateFailed = "Failed to update subscription plan";
        }
    }
}
