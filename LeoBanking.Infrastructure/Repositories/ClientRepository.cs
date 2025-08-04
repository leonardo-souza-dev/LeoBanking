using LeoBanking.Domain.Entities;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LeoBanking.Infrastructure.Repositories;

public class ClientRepository(LeoBankingDbContext context) : IClientRepository
{
    public async Task AddAsync(Client client)
    {
        ArgumentNullException.ThrowIfNull(client);
        
        await context.Clients.AddAsync(client);
    }

    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        var entities = await context.Clients
            .Include(c => c.Account)
            .ToListAsync();

        return entities;
    }

    public async Task<Client> GetByAccountNumber(int accountNumber)
    {
        var client = await context.Clients
            .Include(c => c.Account)
            .Where(c => c.AccountNumber == accountNumber).FirstOrDefaultAsync();
        
        ArgumentNullException.ThrowIfNull(client);
        
        return client;
    }
}