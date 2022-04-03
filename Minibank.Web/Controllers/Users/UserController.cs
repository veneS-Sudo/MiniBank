using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserService _userService;

        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<GetUserDto> GetUserById(string id)
        {
            var entity = await _userService.GetByIdAsync(id);
            return _mapper.Map<GetUserDto>(entity);
        }

        [HttpGet("GetUsers")]
        public async Task<List<GetUserDto>> GetAllUsers()
        {
            return (await _userService.GetAllUsersAsync())
                .Select(entity => _mapper.Map<GetUserDto>(entity))
                .ToList();
        }

        [HttpPost("CreateUser")]
        public Task CreateUser(CreateUserDto user)
        {
            return _userService.CreateUserAsync(_mapper.Map<User>(user));
        }

        [HttpPut("UpdateUser/{userId}")]
        public Task UpdateUser(string userId, UpdateUserDto user)
        {
            var targetUser = _mapper.Map<User>(user);
            targetUser.Id = userId;
            
            return _userService.UpdateUserAsync(targetUser);
        }

        [HttpDelete("DeleteUser")]
        public Task DeleteUser(string userId)
        {
            return _userService.DeleteUserAsync(userId);
        }
    }
}