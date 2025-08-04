using LeoBanking.Domain.Entities;

namespace LeoBanking.Infrastructure.Interfaces;

public interface ITransferRepository
{
    Task AddAsync(Transfer transfer);
    Task<IQueryable<Transfer>> GetAllAsync();
}