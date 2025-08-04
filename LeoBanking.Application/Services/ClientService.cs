using LeoBanking.Application.Interfaces;
using LeoBanking.Domain.Entities;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;
using Minerals.StringCases;

namespace LeoBanking.Application.Services;

public class ClientService(
    IClientRepository clientRepository,
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork) : IClientService
{
    public async Task<Client> Create(Client client, decimal balance)
    {
        ArgumentException.ThrowIfNullOrEmpty(client.Name, nameof(client.Name).ToCamelCase());
        
        var account = new Account(balance);

        await using var transaction = await unitOfWork.BeginTransactionAsync();

        try
        {
            await accountRepository.AddAsync(account);
            await unitOfWork.SaveChangesAsync();

            client.SetAccountNumber(account.Number);

            await clientRepository.AddAsync(client);
            await unitOfWork.SaveChangesAsync();

            await transaction.CommitAsync();
            
            return client;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
    public async Task<IEnumerable<Client>> GetAllAsync()
    {
        var clients = await clientRepository.GetAllAsync();
        return clients;
    }

    public async Task<Client> GetByAccountNumber(int accountNumber)
    {
        var client = await clientRepository.GetByAccountNumber(accountNumber);
        return client;
    }
}