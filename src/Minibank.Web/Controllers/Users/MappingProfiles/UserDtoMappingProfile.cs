using AutoMapper;
using Minibank.Core.Domains.Users;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users.MappingProfiles
{
    public class UserDtoMappingProfile : Profile
    {
        public UserDtoMappingProfile()
        {
            CreateMap<User, GetUserDto>().ReverseMap();
            CreateMap<CreateUserDto, User>();
            CreateMap<UpdateUserDto, User>();
        }
    }
}