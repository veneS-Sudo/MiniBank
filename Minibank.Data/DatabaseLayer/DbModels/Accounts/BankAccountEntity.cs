using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minibank.Core.Converters;
using Minibank.Data.DatabaseLayer.DbModels.Transfers;
using Minibank.Data.DatabaseLayer.DbModels.Users;

namespace Minibank.Data.DatabaseLayer.DbModels.Accounts
{
    public class BankAccountEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public double Balance { get; set; }
        public Currency Currency { get; set; }
        public bool IsOpen { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateClose { get; set; }
        
        // Navigation property
        public UserEntity OwnerUser { get; set; } 
        public List<MoneyTransferEntity> MoneyTransfersToAccount { get; set; }
        public List<MoneyTransferEntity> MoneyTransfersFromAccount { get; set; }

        internal class Map : IEntityTypeConfiguration<BankAccountEntity>
        {
            public void Configure(EntityTypeBuilder<BankAccountEntity> builder)
            {
                builder.ToTable("bank_account");

                builder.Property(entity => entity.Id).HasColumnName("id");
                builder.Property(entity => entity.Balance).HasDefaultValue(0).HasColumnName("balance");
                builder.Property(entity => entity.UserId).IsRequired().HasColumnName("user_id");
                builder.Property(entity => entity.Currency).HasConversion(
                    new ValueConverter<Currency, string>(
                        x => x.ToString(),
                        y => Enum.Parse<Currency>(y)
                        )).IsRequired().HasColumnName("currency");
                builder.Property(entity => entity.IsOpen).HasDefaultValue(true).HasColumnName("is_open");
                builder.Property(entity => entity.DateOpen).IsRequired().HasColumnName("date_open");
                builder.Property(entity => entity.DateClose).HasColumnName("date_close");

                builder.HasKey(entity => entity.Id);
                
                builder.HasOne(entity => entity.OwnerUser)
                    .WithMany(entity => entity.BankAccounts)
                    .HasForeignKey(entity => entity.UserId)
                    .OnDelete(DeleteBehavior.Restrict);
            }
        }
    }
}