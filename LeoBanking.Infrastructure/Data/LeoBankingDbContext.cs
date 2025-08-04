using LeoBanking.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LeoBanking.Infrastructure.Data;

public sealed class LeoBankingDbContext : DbContext
{
    public LeoBankingDbContext(DbContextOptions<LeoBankingDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Number);
            entity.Property(e => e.Number).ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Client>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(c => c.Account)
                .WithOne()
                .HasForeignKey<Client>(c => c.AccountNumber)
                .HasPrincipalKey<Account>(a => a.Number)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Transfer>(entity =>
        {
            entity.HasKey(e => e.Id);
        });
    }
}
