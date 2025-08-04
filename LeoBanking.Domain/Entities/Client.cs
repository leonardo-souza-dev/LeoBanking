namespace LeoBanking.Domain.Entities;

public class Client
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public int AccountNumber { get; private set; }

    public Account Account { get; set; }
    
    public void SetAccountNumber(int accountNumber)
    {
        AccountNumber = accountNumber;
    }
}

