using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task<GetUserDto> GetUserById(string id, CancellationToken cancellationToken)
        {
            var entity = await _userService.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<GetUserDto>(entity);
        }

        [HttpGet("GetUsers")]
        public async Task<List<GetUserDto>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
                
            return users.Select(entity => _mapper.Map<GetUserDto>(entity))
                .ToList();
        }

        [HttpPost("CreateUser")]
        public Task CreateUser(CreateUserDto user, CancellationToken cancellationToken)
        {
            var targetUser = _mapper.Map<User>(user);
            return _userService.CreateUserAsync(targetUser, cancellationToken);
        }

        [HttpPut("UpdateUser/{userId}")]
        public Task UpdateUser(string userId, UpdateUserDto user, CancellationToken cancellationToken)
        {
            var targetUser = _mapper.Map<User>(user);
            targetUser.Id = userId;
            
            return _userService.UpdateUserAsync(targetUser, cancellationToken);
        }

        [HttpDelete("DeleteUser")]
        public Task DeleteUser(string userId, CancellationToken cancellationToken)
        {
            return _userService.DeleteUserAsync(userId, cancellationToken);
        }
    }
}