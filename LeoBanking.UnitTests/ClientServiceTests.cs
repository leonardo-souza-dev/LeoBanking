using LeoBanking.Application.Services;
using LeoBanking.Domain.Entities;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;

namespace LeoBanking.UnitTests;

public class ClientServiceTests
{
     private ClientService _sut;
     private Mock<IClientRepository> _clientRepositoryMock;
     private Mock<IAccountRepository> _accountRepositoryMock;
     private Mock<IDbContextTransaction> _transactionMock;
     private Mock<IUnitOfWork> _unitOfWorkMock;

     [SetUp]
     public void Setup()
     {
         _clientRepositoryMock = new Mock<IClientRepository>();
         _accountRepositoryMock = new Mock<IAccountRepository>();
         _unitOfWorkMock = new Mock<IUnitOfWork>();
         _transactionMock = new Mock<IDbContextTransaction>();
         
         _unitOfWorkMock.Setup(x => x.BeginTransactionAsync())
             .ReturnsAsync(_transactionMock.Object);
         
         _sut = new ClientService(_clientRepositoryMock.Object, _accountRepositoryMock.Object, _unitOfWorkMock.Object);
     }

    [Test]
    public async Task GivenNewClient_WhenValidNameAndBalance_ThenCreateClient()
    {
        // Arrange
        var client = new Client { Name = "John Doe" };
        var balance = 123m;
        
        _accountRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Account>()))
            .Returns(Task.CompletedTask);
        _clientRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Client>()))
            .Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(x => x.SaveChangesAsync(CancellationToken.None))
            .Returns(Task.CompletedTask);
        _transactionMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    
        // Act
        await _sut.Create(client, balance);
    
        // Assert
        _accountRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Account>()), Times.Once());
        _clientRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Client>()), Times.Once());
        _unitOfWorkMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Exactly(2));
        _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once());
    }
    
    [Test]
    public async Task WhenRequestedAllClients_ThenReturnAllClients()
    {
        // Arrange
        var clients = new List<Client> { new() { Name = "Jane Doe" }, new() { Name = "John Doe" } };
        
        _clientRepositoryMock.Setup(x => x.GetAllAsync())
            .ReturnsAsync(clients);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.That(result, Is.EquivalentTo(clients));
        Assert.That(result.Count(), Is.EqualTo(clients.Count));
    }
    
    [Test]
    public async Task WhenRequestedClientByAccountNumber_ThenReturnClient()
    {
        // Arrange
        var accountNumber = 12345;
        var client = new Client { Name = "John Doe" };
        client.SetAccountNumber(accountNumber);
        
        _clientRepositoryMock.Setup(x => x.GetByAccountNumber(accountNumber))
            .ReturnsAsync(client);

        // Act
        var result = await _sut.GetByAccountNumber(accountNumber);

        // Assert
        Assert.That(result, Is.EqualTo(client));
        _clientRepositoryMock.Verify(x => x.GetByAccountNumber(accountNumber), Times.Once());
    }
    
    [Test]
    public void GivenNewClient_WhenExceptionOccurs_ThenRollbackTransaction()
    {
        // Arrange
        var client = new Client { Name = "John Doe" };
        var balance = 123m;
        
        _accountRepositoryMock.Setup(x => x.AddAsync(It.IsAny<Account>()))
            .ThrowsAsync(new Exception("Database error"));
        _transactionMock.Setup(x => x.RollbackAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
    
        // Act & Assert
        var exception = Assert.ThrowsAsync<Exception>(async () => await _sut.Create(client, balance));
        
        _transactionMock.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once());
        _transactionMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never());
    }
}