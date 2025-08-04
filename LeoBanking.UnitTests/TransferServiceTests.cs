using LeoBanking.Application.Services;
using LeoBanking.Domain.Entities;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;
using LeoBanking.Shared.Wrappers;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace LeoBanking.UnitTests;

public class TransferServiceTests
{
    private TransferService _sut;
    private Mock<IAccountRepository> _accountRepositoryMock;
    private Mock<ITransferRepository> _transferRepositoryMock;
    private Mock<IConfigurationWrapper> _configurationWrapperMock;
    private Mock<IUnitOfWork> _unitOfWorkMock;
    private Mock<IDbContextTransaction> _transactionMock;

    [SetUp]
    public void Setup()
    {
        _accountRepositoryMock = new Mock<IAccountRepository>();
        _transferRepositoryMock = new Mock<ITransferRepository>();
        _configurationWrapperMock = new Mock<IConfigurationWrapper>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _transactionMock = new Mock<IDbContextTransaction>();

        _unitOfWorkMock.Setup(x => x.BeginTransactionAsync()).ReturnsAsync(_transactionMock.Object);

        _sut = new TransferService(_accountRepositoryMock.Object, _transferRepositoryMock.Object, _configurationWrapperMock.Object, _unitOfWorkMock.Object);
    }

    [Test]
    public async Task GivenValidTransfer_WhenAmountIsValid_ThenCreateTransfer()
    {
        // Arrange
        _configurationWrapperMock.Setup(x => x.GetValue("TRANSFER_MIN_VALUE")).Returns("0.01");
        _configurationWrapperMock.Setup(x => x.GetValue("TRANSFER_MAX_VALUE")).Returns("10000");

        var account1 = new Account(123) { Number = 1 };
        var account2 = new Account(1) { Number = 2 };

        _accountRepositoryMock.Setup(x => x.FindAsync(1)).ReturnsAsync(account1);
        _accountRepositoryMock.Setup(x => x.FindAsync(2)).ReturnsAsync(account2);
        _accountRepositoryMock.Setup(x => x.Update(It.IsAny<Account>())).Returns(Task.CompletedTask);
        _transferRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Transfer>())).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(CancellationToken.None)).Returns(Task.CompletedTask);
        _transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var transfer = new Transfer
        {
            OriginAccountNumber = 1,
            DestinationAccountNumber = 2,
            Amount = 100
        };

        // Act
        await _sut.Create(transfer);

        // Assert
        _accountRepositoryMock.Verify(x => x.Update(It.IsAny<Account>()), Times.Exactly(2));
        _transferRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Transfer>()), Times.Once());
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once());
        _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
    }

    [Test]
    public void GivenTransfer_WhenAmountBelowMinimum_ThenThrowArgumentOutOfRangeException()
    {
        // Arrange
        _configurationWrapperMock.Setup(x => x.GetValue("TRANSFER_MIN_VALUE")).Returns("10");
        _configurationWrapperMock.Setup(x => x.GetValue("TRANSFER_MAX_VALUE")).Returns("10000");

        var transfer = new Transfer
        {
            OriginAccountNumber = 1,
            DestinationAccountNumber = 2,
            Amount = 5
        };

        // Act & Assert
        var exception = Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await _sut.Create(transfer));
        Assert.That(exception.ParamName, Is.EqualTo("amount"));
    }

    [Test]
    public void GivenTransfer_WhenExceptionOccurs_ThenRollbackTransaction()
    {
        // Arrange
        _configurationWrapperMock.Setup(x => x.GetValue("TRANSFER_MIN_VALUE")).Returns("0.01");
        _configurationWrapperMock.Setup(x => x.GetValue("TRANSFER_MAX_VALUE")).Returns("10000");

        var account1 = new Account(123) { Number = 1 };
        var account2 = new Account(1) { Number = 2 };

        _accountRepositoryMock.Setup(x => x.FindAsync(1)).ReturnsAsync(account1);
        _accountRepositoryMock.Setup(x => x.FindAsync(2)).ReturnsAsync(account2);
        _accountRepositoryMock.Setup(x => x.Update(It.IsAny<Account>())).ThrowsAsync(new Exception("Database error"));
        _transactionMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var transfer = new Transfer
        {
            OriginAccountNumber = 1,
            DestinationAccountNumber = 2,
            Amount = 100
        };

        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () => await _sut.Create(transfer));

        _transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
        _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
    }
    
    [Test]
    public async Task GetAllAsync_WhenTransfersExist_ThenReturnsTransfersOrderedByCreatedAtDesc()
    {
        // Arrange
        var firstTransfer = new Transfer 
        { 
            OriginAccountNumber = 1001, 
            DestinationAccountNumber = 1002, 
            Amount = 100, 
            CreatedAt = new DateTime(2023, 1, 1) 
        };
        
        var secondTransfer = new Transfer 
        { 
            OriginAccountNumber = 1003, 
            DestinationAccountNumber = 1004, 
            Amount = 75, 
            CreatedAt = new DateTime(2023, 6, 1) 
        };
        
        var lastTransfer = new Transfer 
        { 
            OriginAccountNumber = 1002, 
            DestinationAccountNumber = 1003, 
            Amount = 50, 
            CreatedAt = new DateTime(2024, 1, 1) 
        };
        
        var transfers = new List<Transfer> { firstTransfer, lastTransfer, secondTransfer }.AsQueryable();
        
        _transferRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transfers);

        var accountNumberToGet = 1002;

        // Act
        var result = await _sut.GetAllByAccountNumberAsync(accountNumberToGet);
        var resultList = result.ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(resultList.Count, Is.EqualTo(2));
        
        Assert.That(resultList[0].CreatedAt, Is.EqualTo(new DateTime(2024, 1, 1)));
        Assert.That(resultList[1].CreatedAt, Is.EqualTo(new DateTime(2023, 1, 1)));
        
        _transferRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
    
    [Test]
    public async Task GetAllAsync_WhenNoTransfers_ThenReturnsEmptyList()
    {
        // Arrange
        var noTransfers = new List<Transfer>().AsQueryable();
        _transferRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(noTransfers);

        // Act
        var result = await _sut.GetAllByAccountNumberAsync(1000);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(0));
        _transferRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
    
    [Test]
    public async Task GetAllAsync_WhenOneTransfer_ThenReturnsOneTransfer()
    {
        // Arrange
        var transfer = new Transfer 
        { 
            OriginAccountNumber = 1001, 
            DestinationAccountNumber = 1002, 
            Amount = 100, 
            CreatedAt = new DateTime(2024, 1, 1) 
        };
        
        var transfers = new List<Transfer> { transfer }.AsQueryable();
        _transferRepositoryMock.Setup(x => x.GetAllAsync()).ReturnsAsync(transfers);
        
        var accountNumberToGet = 1002;

        // Act
        var result = await _sut.GetAllByAccountNumberAsync(accountNumberToGet);
        var resultList = result.ToList();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(resultList.Count, Is.EqualTo(1));
        Assert.That(resultList[0].OriginAccountNumber, Is.EqualTo(1001));
        Assert.That(resultList[0].DestinationAccountNumber, Is.EqualTo(1002));
        Assert.That(resultList[0].Amount, Is.EqualTo(100));
        Assert.That(resultList[0].CreatedAt, Is.EqualTo(new DateTime(2024, 1, 1)));
        _transferRepositoryMock.Verify(x => x.GetAllAsync(), Times.Once);
    }
}