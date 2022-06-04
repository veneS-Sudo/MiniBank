using AutoMapper;
using Minibank.Core.Domains.Users;

namespace Minibank.Data.DatabaseLayer.DbModels.Users.MappingProfiles
{
    public class UserEntityProfile : Profile
    {
        public UserEntityProfile()
        {
            CreateMap<UserEntity, User>().ReverseMap();
        }
    }
}