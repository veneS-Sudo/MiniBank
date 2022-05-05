using AutoMapper;
using Minibank.Core.Domains.Accounts;
using Minibank.Web.Controllers.Accounts.Dto;

namespace Minibank.Web.Controllers.Accounts.MappingProfiles
{
    public class BankAccountDtoProfile : Profile
    {
        public BankAccountDtoProfile()
        {
            CreateMap<BankAccount, GetBankAccountDto>();
            CreateMap<CreateBankAccountDto, BankAccount>();
            CreateMap<UpdateAccountDto, BankAccount>();
        }
    }
}