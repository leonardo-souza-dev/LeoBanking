using LeoBanking.Api.Dtos;
using LeoBanking.Application.Interfaces;

namespace LeoBanking.Api;

public static class Endpoints
{
    public static void MapClientEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/v1/clients", async (CreateClientRequest createClient, IClientService service) =>
        {
            try
            {
                var entity = await service.Create(createClient.ToEntity(), createClient.Balance);
                return Results.Created($"/v1/clients/{entity.Id}", GetClientResponse.Of(entity));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: 500, detail: ex.Message);
            }
        });

        app.MapGet("/v1/clients", async (IClientService service) =>
        {
            try
            {
                var entities = await service.GetAllAsync();
                return Results.Ok(GetClientsResponse.Of(entities));
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: 500, detail: ex.Message);
            }
        });

        app.MapGet("/v1/clients/{accountNumber:int}", async (int accountNumber, IClientService service) =>
        {
            try
            {
                var entity = await service.GetByAccountNumber(accountNumber);
                return Results.Ok(GetClientResponse.Of(entity));
            }
            catch (ArgumentNullException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: 500, detail: ex.Message);
            }
        });

        app.MapPost("/v1/transfers", async (CreateTransferRequest createTransfer, ITransferService service) =>
        {
            try
            {
                var entity = await service.Create(createTransfer.ToEntity());
                return Results.Created($"/v1/transfers/{entity.Id}", TransferResponse.Of(entity));
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Results.UnprocessableEntity(ex.Message);
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: 500, detail: ex.Message);
            }
        });

        app.MapGet("/v1/transfers/{accountNumber:int}", async (int accountNumber, ITransferService service) =>
        {
            try
            {
                var entities = await service.GetAllByAccountNumberAsync(accountNumber);
                var response = GetTransfersResponse.Of(entities);
                return Results.Ok(response);
            }
            catch (Exception ex)
            {
                return Results.Problem(statusCode: 500, detail: ex.Message);
            }
        });
    }
}