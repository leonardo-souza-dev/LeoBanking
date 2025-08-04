using LeoBanking.Application.Interfaces;
using LeoBanking.Domain.Entities;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;
using LeoBanking.Shared.Wrappers;
using Minerals.StringCases;

namespace LeoBanking.Application.Services;

public class TransferService(
    IAccountRepository accountRepository, 
    ITransferRepository transferRepository,
    IConfigurationWrapper config,
    IUnitOfWork unitOfWork) : ITransferService
{
    private const string MIN_TRANSFER_VALUE_CONFIG_KEY = "TRANSFER_MIN_VALUE";
    private const string MAX_TRANSFER_VALUE_CONFIG_KEY = "TRANSFER_MAX_VALUE";
    
    public async Task<Transfer> Create(Transfer transfer)
    {
        var minTransferValue = decimal.Parse(config.GetValue(MIN_TRANSFER_VALUE_CONFIG_KEY));
        var maxTransferValue = decimal.Parse(config.GetValue(MAX_TRANSFER_VALUE_CONFIG_KEY));

        if (transfer.Amount < minTransferValue || transfer.Amount > maxTransferValue)
        {
            transfer.SetSuccess(false);
            await transferRepository.AddAsync(transfer);
            await unitOfWork.SaveChangesAsync();
            
            throw new ArgumentOutOfRangeException(
                nameof(transfer.Amount).ToCamelCase(),
                $"Amount must be between {minTransferValue} and {maxTransferValue}");
        }

        var originAccount = await accountRepository.FindAsync(transfer.OriginAccountNumber);
        if (originAccount == null)
        {
            await PersistUnsuccessfulTransfer(transfer);
            ArgumentNullException.ThrowIfNull(originAccount, nameof(transfer.OriginAccountNumber).ToCamelCase());
        }
        
        var destinationAccount = await accountRepository.FindAsync(transfer.DestinationAccountNumber);
        if (destinationAccount == null)
        {
            await PersistUnsuccessfulTransfer(transfer);
            ArgumentNullException.ThrowIfNull(destinationAccount, nameof(transfer.DestinationAccountNumber).ToCamelCase());
        }

        if (!originAccount.IsAllowedToMakeTransfersOf(transfer.Amount))
        {
            await PersistUnsuccessfulTransfer(transfer);
            throw new InvalidOperationException("Origin account is not allowed to do this transfer");
        }

        originAccount.SubtractAmount(transfer.Amount);
        destinationAccount.AddAmount(transfer.Amount);

        await using var transaction = await unitOfWork.BeginTransactionAsync();

        try
        {
            await accountRepository.Update(originAccount);
            await accountRepository.Update(destinationAccount);
            
            transfer.SetSuccess(true);
            await transferRepository.AddAsync(transfer);
        
            await unitOfWork.SaveChangesAsync();
            await transaction.CommitAsync();
            
            return transfer;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task PersistUnsuccessfulTransfer(Transfer transfer)
    {
        transfer.SetSuccess(false);
        await transferRepository.AddAsync(transfer);
        await unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<Transfer>> GetAllByAccountNumberAsync(int accountNumber)
    {
        var transfers = await transferRepository.GetAllAsync();
        var transfersOrdered = transfers
            .Where(t => t.OriginAccountNumber == accountNumber || t.DestinationAccountNumber == accountNumber)
            .OrderByDescending(t => t.CreatedAt);
        return transfersOrdered;
    }
}