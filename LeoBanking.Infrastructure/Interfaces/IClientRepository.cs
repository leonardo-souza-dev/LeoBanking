using LeoBanking.Domain.Entities;

namespace LeoBanking.Infrastructure.Interfaces;

public interface IClientRepository
{
    Task AddAsync(Client client);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client> GetByAccountNumber(int accountNumber);
}