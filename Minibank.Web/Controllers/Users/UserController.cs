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
        public IEnumerable<UserDto> GetAllUsers()
        {
            return _userService.GetAllUsers().Select(entity => new UserDto
            {
                Id = entity.Id,
                Login = entity.Login,
                Email = entity.Email
            });
        }

        [HttpPost]
        public void CreateUser(UserDto user)
        {
            _userService.CreateUser(new User
            {
                Id = user.Id,
                Login = user.Login,
                Email = user.Email
            });
        }

        [HttpPut]
        public void UpdateUser(UserDto user)
        {
            _userService.UpdateUser(new User
            {
                Id = user.Id,
                Login = user.Login,
                Email = user.Email
            });
        }

        [HttpDelete]
        public void DeleteUser(string id)
        {
            _userService.DeleteUser(id);
        }
    }
}