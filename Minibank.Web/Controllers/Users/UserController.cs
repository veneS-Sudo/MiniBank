using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Services;
using Minibank.Web.Controllers.Users.Dto;

namespace Minibank.Web.Controllers.Users
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id}")]
        public UserDto GetUserById(string id)
        {
            var entity = _userService.GetById(id);
            return new UserDto
            {
                Id = entity.Id,
                Login = entity.Login,
                Email = entity.Email
            };
        }

        [HttpGet]
        public List<UserDto> GetAllUsers()
        {
            return _userService.GetAllUsers().Select(entity => new UserDto
            {
                Id = entity.Id,
                Login = entity.Login,
                Email = entity.Email
            }).ToList();
        }

        [HttpPost]
        public void CreateUser(CreateUserDto user)
        {
            _userService.CreateUser(new User
            {
                Login = user.Login,
                Email = user.Email
            });
        }

        [HttpPut("{userId}")]
        public void UpdateUser(string userId, UpdateUserDto user)
        {
            _userService.UpdateUser(new User
            {
                Id = userId,
                Login = user.Login,
                Email = user.Email
            });
        }

        [HttpDelete]
        public void DeleteUser(string userId)
        {
            _userService.DeleteUser(userId);
        }
    }
}