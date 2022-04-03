using AutoMapper;
using Minibank.Core.Domains.Transfers;

namespace Minibank.Data.DatabaseLayer.DbModels.Transfers.MappingProfiles
{
    public class MoneyTransferEntityProfile : Profile
    {
        public MoneyTransferEntityProfile()
        {
            CreateMap<MoneyTransferEntity, MoneyTransfer>().ReverseMap();
        }
    }
}