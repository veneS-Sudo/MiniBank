using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minibank.Core.Converters;
using Minibank.Data.DatabaseLayer.DbModels.Accounts;

namespace Minibank.Data.DatabaseLayer.DbModels.Transfers
{
    public class MoneyTransferEntity
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public string FromBankAccountId { get; set; }
        public string ToBankAccountId { get; set; }
        
        // Navigation property
        public BankAccountEntity FromBankAccount { get; set; }
        public BankAccountEntity ToBankAccount { get; set; }

        internal class Map : IEntityTypeConfiguration<MoneyTransferEntity>
        {
            public void Configure(EntityTypeBuilder<MoneyTransferEntity> builder)
            {
                builder.ToTable("money_transfer");

                builder.Property(entity => entity.Id);
                builder.Property(entity => entity.Amount).IsRequired();
                builder.Property(entity => entity.Currency).HasConversion(
                    new ValueConverter<Currency, string>(
                        x => x.ToString(),
                        y => Enum.Parse<Currency>(y)
                        )).IsRequired();
                builder.Property(entity => entity.FromBankAccountId).IsRequired();
                builder.Property(entity => entity.ToBankAccountId).IsRequired();

                builder.HasKey(entity => entity.Id);
                
                builder.HasOne(entity => entity.FromBankAccount)
                    .WithMany(entity => entity.MoneyTransfersFromAccount)
                    .HasForeignKey(entity => entity.FromBankAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
                
                builder.HasOne(entity => entity.ToBankAccount)
                    .WithMany(entity => entity.MoneyTransfersToAccount)
                    .HasForeignKey(entity => entity.ToBankAccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}