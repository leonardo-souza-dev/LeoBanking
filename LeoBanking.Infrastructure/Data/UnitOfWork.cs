using Microsoft.EntityFrameworkCore.Storage;

namespace LeoBanking.Infrastructure.Data;

public sealed class UnitOfWork(LeoBankingDbContext context) : IUnitOfWork
{
    public Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return context.Database.BeginTransactionAsync();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return context.SaveChangesAsync(cancellationToken);
    }
}
