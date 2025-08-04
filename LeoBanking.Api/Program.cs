using LeoBanking.Api;
using LeoBanking.Application.Interfaces;
using LeoBanking.Application.Services;
using LeoBanking.Infrastructure.Data;
using LeoBanking.Infrastructure.Interfaces;
using LeoBanking.Infrastructure.Repositories;
using LeoBanking.Shared.Wrappers;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<ITransferService, TransferService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransferRepository, TransferRepository>();
builder.Services.AddScoped<IConfigurationWrapper, ConfigurationWrapper>();
var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
builder.Services.AddDbContext<LeoBankingDbContext>(options => options.UseSqlite(connection));

var app = builder.Build();
app.UsePathBase("/api");
app.MapClientEndpoints();
app.Run();

public partial class Program { }

