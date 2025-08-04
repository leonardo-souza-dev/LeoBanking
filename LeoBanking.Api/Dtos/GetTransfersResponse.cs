using LeoBanking.Domain.Entities;

namespace LeoBanking.Api.Dtos;

public class GetTransfersResponse : List<TransferResponse>
{
    private GetTransfersResponse(IEnumerable<TransferResponse> items) =>
        AddRange(items);

    public static GetTransfersResponse Of(IEnumerable<Transfer> entities) =>
        new (entities.Select(TransferResponse.Of));

    public bool Any() => Count > 0;
}
