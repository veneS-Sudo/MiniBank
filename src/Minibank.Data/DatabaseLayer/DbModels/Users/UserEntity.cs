using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Minibank.Data.DatabaseLayer.DbModels.Accounts;

namespace Minibank.Data.DatabaseLayer.DbModels.Users
{
    public class UserEntity
    {
        public string Id { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        
        public List<BankAccountEntity> BankAccounts { get; set; }
        internal class Map : IEntityTypeConfiguration<UserEntity>
        {
            public void Configure(EntityTypeBuilder<UserEntity> builder)
            {
                builder.ToTable("user");
                
                builder.Property(entity => entity.Id);
                builder.Property(entity => entity.Login).IsRequired();
                builder.Property(entity => entity.Email).IsRequired();
                
                builder.HasKey(entity => entity.Id);
            }
        }
    }
}