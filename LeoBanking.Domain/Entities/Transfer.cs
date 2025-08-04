namespace LeoBanking.Domain.Entities;

public class Transfer
{
    public int Id { get; init; }
    public int OriginAccountNumber { get; init; }
    public int DestinationAccountNumber { get; init; }
    public decimal Amount { get; init; }
    public bool Success { get; private set; }
    public DateTime CreatedAt { get; init; } = DateTime.Now;
    
    public void SetSuccess(bool success) => Success = success;
}
