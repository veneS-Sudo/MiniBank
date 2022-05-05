using AutoMapper;
using Minibank.Core.Domains.Accounts;

namespace Minibank.Data.DatabaseLayer.DbModels.Accounts.MappingProfiles
{
    public class BankAccountEntityProfile : Profile
    {
        public BankAccountEntityProfile()
        {
            CreateMap<BankAccount, BankAccountEntity>().ReverseMap();
        }
    }
}