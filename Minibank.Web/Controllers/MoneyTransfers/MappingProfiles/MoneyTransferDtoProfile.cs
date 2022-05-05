using AutoMapper;
using Minibank.Core.Domains.Transfers;
using Minibank.Web.Controllers.MoneyTransfers.Dto;

namespace Minibank.Web.Controllers.MoneyTransfers.MappingProfiles
{
    public class MoneyTransferDtoProfile : Profile
    {
        public MoneyTransferDtoProfile()
        {
            CreateMap<CreateMoneyTransferDto, MoneyTransfer>();
        }
    }
}