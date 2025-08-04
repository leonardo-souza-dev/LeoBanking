using System.Text.Json.Serialization;
using LeoBanking.Domain.Entities;

namespace LeoBanking.Api.Dtos;

public class CreateClientRequest
{
    public required string Name { get; init; }
    public required decimal Balance { get; init; }

    public Client ToEntity() => new() { Name = this.Name };
}
