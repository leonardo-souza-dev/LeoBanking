using Microsoft.EntityFrameworkCore.Storage;

namespace LeoBanking.Infrastructure.Data;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(); 
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
