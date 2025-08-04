using LeoBanking.Domain.Entities;

namespace LeoBanking.Application.Interfaces;

public interface ITransferService
{
    Task<Transfer> Create(Transfer transfer);
    Task<IEnumerable<Transfer>> GetAllByAccountNumberAsync(int accountNumber);
}
