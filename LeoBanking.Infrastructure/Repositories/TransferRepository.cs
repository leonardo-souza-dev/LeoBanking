using LeoBanking.Domain.Entities;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;

namespace LeoBanking.Infrastructure.Repositories;

public class TransferRepository(LeoBankingDbContext context) : ITransferRepository
{
    public async Task AddAsync(Transfer transfer)
    {
        ArgumentNullException.ThrowIfNull(transfer);
        
        await context.Transfers.AddAsync(transfer);
    }

    public async Task<IQueryable<Transfer>> GetAllAsync() =>
        await Task.FromResult(context.Transfers.AsQueryable());
}