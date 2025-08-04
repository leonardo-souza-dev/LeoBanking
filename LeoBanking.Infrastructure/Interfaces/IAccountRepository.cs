using LeoBanking.Domain.Entities;

namespace LeoBanking.Infrastructure.Interfaces;

public interface IAccountRepository
{
    Task<Account?> FindAsync(int accountNumber);
    Task AddAsync(Account account);
    Task Update(Account account);
    
}