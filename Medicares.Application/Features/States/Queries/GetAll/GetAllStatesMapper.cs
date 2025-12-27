using Medicares.Domain.Entities.Common;
namespace Medicares.Application.Features.States.Queries.GetAll
{
    public static class GetAllStatesMapper
    {
        public static List<GetAllStatesResponse> MapToGetAllStatesQueryResponse(this IEnumerable<State> responses)
        {
            return responses.Select(response => new GetAllStatesResponse
            {
                Id = response.Id,
                Name = response.Name,
                ShortName = response.ShortName,
            }).ToList();
        }
    }
}
