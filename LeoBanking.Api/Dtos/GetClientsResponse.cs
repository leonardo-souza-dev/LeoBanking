using LeoBanking.Domain.Entities;

namespace LeoBanking.Api.Dtos;

public class GetClientsResponse : List<GetClientResponse>
{
    private GetClientsResponse(IEnumerable<GetClientResponse> items) =>
        AddRange(items);

    public static GetClientsResponse Of(IEnumerable<Client> entities) =>
        new (entities.Select(GetClientResponse.Of));

    public bool Any() => Count > 0;
}
