using System.ComponentModel.DataAnnotations.Schema;

namespace LeoBanking.Domain.Entities;

public class Account(decimal balance)
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Number { get; init; }
    public decimal Balance { get; private set; } = balance;

    public void AddAmount(decimal amount) => Balance += amount;
    
    public void SubtractAmount(decimal amount)
    {
        if (Balance < 0)
            throw new ArgumentOutOfRangeException("Balance must be greater than 0");
        
        if (amount < 0)
            throw new ArgumentOutOfRangeException("Amount must be greater than 0");
            
        Balance -= amount;
    }

    public bool IsAllowedToMakeTransfersOf(decimal amount) => Balance > 0 && Balance >= amount;
}
