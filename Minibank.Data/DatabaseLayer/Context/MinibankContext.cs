using Microsoft.EntityFrameworkCore;
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

        public MinibankContext(DbContextOptions<MinibankContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UserEntity).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql().UseSnakeCaseNamingConvention();
            base.OnConfiguring(optionsBuilder);
        }
    }
}