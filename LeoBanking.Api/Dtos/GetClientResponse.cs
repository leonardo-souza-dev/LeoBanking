using LeoBanking.Domain.Entities;

namespace LeoBanking.Api.Dtos;

public class GetClientResponse
{
    public required int Id { get; init; }
    public required string Name { get; init; }
    public required int AccountNumber { get; init; }
    public required decimal AccountBalance { get; init; }

    public static GetClientResponse Of(Client entity)
    {
        return new GetClientResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            AccountNumber = entity.AccountNumber,
            AccountBalance = entity.Account.Balance
        };
    }
}
