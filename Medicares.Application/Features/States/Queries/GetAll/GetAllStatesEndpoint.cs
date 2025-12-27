using FastEndpoints;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Application.Features.States.Queries.GetAll
{
    public class GetAllStatesEndpoint(IUnitOfWork unitOfWork) : EndpointWithoutRequest<List<GetAllStatesResponse>>
    {
        public override void Configure()
        {
            Get(StatesApiRoutes.GetAllStates);

            AllowAnonymous();

            Summary(s =>
            {
                s.Summary = StatesApiRoutes.Summary;
            });

        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            List<State> states = await unitOfWork.Repository<State>().Entities.ToListAsync(ct);
            await SendOkAsync(states.MapToGetAllStatesQueryResponse(), ct);
        }
    }
}
