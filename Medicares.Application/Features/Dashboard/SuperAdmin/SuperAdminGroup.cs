using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace Medicares.Application.Features.Dashboard.SuperAdmin
{
    public class SuperAdminGroup : Group
    {
        public SuperAdminGroup()
        {
            base.Configure(
                "super-admin",
                ep =>
                {
                    ep.Description(
                        x => x.Produces(401)
                              .WithTags("Super-Admin"));
                });
        }

        public static class ApiRoutes
        {
            public const string GetAllOwners = "owners/get-all";
            public const string GetStats = "stats";
        }

        public static class SuperAdminConsts
        {
            public const string GetAllOwnersSummary = "Get All Registered Owners";
            public const string GetAllOwnersDescription = "Endpoint to fetch all owners registered in the system.";
        }
    }
}
