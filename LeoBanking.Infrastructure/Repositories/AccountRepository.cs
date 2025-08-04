using LeoBanking.Domain.Entities;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;

namespace LeoBanking.Infrastructure.Repositories;

public class AccountRepository(LeoBankingDbContext context) : IAccountRepository
{
    public async Task<Account?> FindAsync(int accountNumber)
    {
        var account = await context.Accounts.FindAsync(accountNumber);
        return account;
    }
    
    public async Task AddAsync(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);
        
        await context.Accounts.AddAsync(account);
    }

    public async Task Update(Account account)
    {
        ArgumentNullException.ThrowIfNull(account);

        context.Accounts.Update(account);
        await Task.CompletedTask;
    }
}