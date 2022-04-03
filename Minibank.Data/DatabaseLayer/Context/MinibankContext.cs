using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Minibank.Data.DatabaseLayer.DbModels.Accounts;
using Minibank.Data.DatabaseLayer.DbModels.Transfers;
using Minibank.Data.DatabaseLayer.DbModels.Users;

namespace Minibank.Data.DatabaseLayer.Context
{
    public class MinibankContext : DbContext
    {
        public DbSet<UserEntity> Users { get; set; }
        public DbSet<BankAccountEntity> BankAccounts { get; set; }
        public DbSet<MoneyTransferEntity> AmountTransfers { get; set; }

        public MinibankContext(DbContextOptions options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntity).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Factory : IDesignTimeDbContextFactory<MinibankContext>
    {
        public MinibankContext CreateDbContext(string[] args)
        {
            var option = new DbContextOptionsBuilder()
                .UseNpgsql("Username=postgres;Password=123456;Host=localhost;Port=5432;Database=minibank")
                .Options;

            return new MinibankContext(option);
        }
    }
}