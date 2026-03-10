using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Application.Contracts.Wrappers;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Shared.Constant;

namespace Medicares.Application.Features.Dashboard.SuperAdmin.GetAllOwners
{
    public class GetAllOwnersEndpoint(IUnitOfWork unitOfWork) : EndpointWithoutRequest<Result<List<GetAllOwnersResponse>>>
    {
        public override void Configure()
        {
            Get(SuperAdminGroup.ApiRoutes.GetAllOwners);
            Group<SuperAdminGroup>();
            Roles(RoleConsts.SuperAdmin);
            Summary(s =>
            {
                s.Summary = SuperAdminGroup.SuperAdminConsts.GetAllOwnersSummary;
                s.Description = SuperAdminGroup.SuperAdminConsts.GetAllOwnersDescription;
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            GetAllOwnersSpecification spec = new GetAllOwnersSpecification();
            List<Owner> owners = await unitOfWork.Repository<Owner>()
                .GetWithSpecificationAsync(spec, ct);

            List<GetAllOwnersResponse> response = owners.MapToResponse();

            await SendOkAsync(await Result<List<GetAllOwnersResponse>>.SuccessAsync(response), ct);
        }
    }
}
