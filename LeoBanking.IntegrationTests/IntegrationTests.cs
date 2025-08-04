using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;
using LeoBanking.Api;
using LeoBanking.Api.Dtos;
using LeoBanking.Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;

namespace LeoBanking.IntegrationTests;

public class IntegrationTests
{
    private HttpClient _client = null!;
    private WebApplicationFactory<Program> _factory = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }
    
    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task GivenCreatingTwoClients_WhenTransfer_ThenShouldTransfer()
    {
        // Arrange
        
        // Act & Assert
        var createClient1Response = await _client.PostAsJsonAsync("/v1/clients", new CreateClientRequest
        {
            Name = "John",
            Balance = 123.00m
        });
        Assert.That(createClient1Response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var client1 = await createClient1Response.Content.ReadFromJsonAsync<GetClientResponse>();
        Assert.That(client1, Is.Not.Null);
        
        var createClient2Response = await _client.PostAsJsonAsync("/v1/clients", new CreateClientRequest
        {
            Name = "Bob",
            Balance = 1.00m
        });
        Assert.That(createClient2Response.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var client2 = await createClient2Response.Content.ReadFromJsonAsync<GetClientResponse>();
        Assert.That(client2, Is.Not.Null);
        
        var transferResponse = await _client.PostAsJsonAsync("/v1/transfers", new CreateTransferRequest
        {
            OriginAccountNumber = client1.AccountNumber,
            DestinationAccountNumber = client2.AccountNumber,
            Amount = 10.00m
        });
        Assert.That(transferResponse.StatusCode, Is.EqualTo(HttpStatusCode.Created));
        
        var clients = await _client.GetFromJsonAsync<List<GetClientResponse>>("/v1/clients");
        Assert.That(clients, Is.Not.Null);
        Assert.That(clients, Has.Count.EqualTo(2));

        // Assert
    }
}
