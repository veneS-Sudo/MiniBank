using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users
{
    [ApiController]
    [Authorize]
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

        [HttpGet("[action]")]
        public async Task<GetUserDto> GetUserById(string id, CancellationToken cancellationToken)
        {
            var entity = await _userService.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<GetUserDto>(entity);
        }

        [HttpGet("[action]")]
        public async Task<List<GetUserDto>> GetAllUsers(CancellationToken cancellationToken)
        {
            var users = await _userService.GetAllUsersAsync(cancellationToken);
                
            return _mapper.Map<List<User>, List<GetUserDto>>(users);
        }

        [HttpPost("[action]")]
        public async Task<string> CreateUser(CreateUserDto user, CancellationToken cancellationToken)
        {
            var targetUser = _mapper.Map<User>(user);
            var createUser = await _userService.CreateUserAsync(targetUser, cancellationToken);
            return createUser.Id;
        }

        [HttpPut("[action]/{userId}")]
        public async Task<bool> UpdateUser(string userId, UpdateUserDto user, CancellationToken cancellationToken)
        {
            var targetUser = _mapper.Map<User>(user);
            targetUser.Id = userId;
            
            var userUpdated = await _userService.UpdateUserAsync(targetUser, cancellationToken);
            return userUpdated;
        }

        [HttpDelete("[action]/{userId}")]
        public async Task<bool> DeleteUser(string userId, CancellationToken cancellationToken)
        {
            return await _userService.DeleteUserAsync(userId, cancellationToken);
        }
    }
}