using LeoBanking.Domain.Entities;

namespace LeoBanking.Api.Dtos;

public class CreateTransferRequest
{
    public required int OriginAccountNumber { get; set; }
    public required int DestinationAccountNumber { get; set; }
    public required decimal Amount { get; set; }

    public Transfer ToEntity() =>
        new()
        {
            OriginAccountNumber = this.OriginAccountNumber,
            DestinationAccountNumber = this.DestinationAccountNumber,
            Amount = this.Amount
        };
}
