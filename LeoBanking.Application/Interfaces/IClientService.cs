using LeoBanking.Domain.Entities;

namespace LeoBanking.Application.Interfaces;

public interface IClientService
{
    Task<Client> Create(Client client, decimal balance);
    Task<IEnumerable<Client>> GetAllAsync();
    Task<Client> GetByAccountNumber(int accountNumber);
}
