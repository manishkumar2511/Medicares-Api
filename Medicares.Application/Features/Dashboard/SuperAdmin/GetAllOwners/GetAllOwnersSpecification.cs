using Medicares.Application.Contracts.Specifications;
using Medicares.Domain.Entities.Auth;

namespace Medicares.Application.Features.Dashboard.SuperAdmin.GetAllOwners
{
    public class GetAllOwnersSpecification : BaseSpecification<Owner>
    {
        public GetAllOwnersSpecification()
        {
            AddInclude(x => x.User);
            AddInclude("User.Address.State");
            OrderByDescending = "CreatedAt";
        }
    }
}
