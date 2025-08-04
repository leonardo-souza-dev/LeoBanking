using LeoBanking.Domain.Entities;

namespace LeoBanking.Api.Dtos;

public class TransferResponse
{
    public int Id { get; init; }
    public int OriginAccountNumber { get; init; }
    public int DestinationAccountNumber { get; init; }
    public decimal Amount { get; init; }
    public bool Success { get; init; }
    public DateTime CreatedAt { get; init; }

    public static TransferResponse Of(Transfer entity)
    {
        return new TransferResponse
        {
            Id = entity.Id,
            OriginAccountNumber = entity.OriginAccountNumber,
            DestinationAccountNumber = entity.DestinationAccountNumber,
            Amount = entity.Amount,
            Success = entity.Success,
            CreatedAt = entity.CreatedAt
        };
    }
}
