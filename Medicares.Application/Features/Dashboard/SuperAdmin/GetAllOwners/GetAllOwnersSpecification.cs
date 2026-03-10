using Medicares.Application.Contracts.Specifications;
using Medicares.Domain.Entities.Auth;

namespace Medicares.Application.Features.Dashboard.SuperAdmin.GetAllOwners
{
    public class GetAllOwnersSpecification : BaseSpecification<Owner>
    {
        public GetAllOwnersSpecification()
        {
            AddInclude(x => x.ApplicationUser);
            AddInclude("ApplicationUser.Address.State");
            OrderByDescending = "CreatedAt";
        }
    }
}
