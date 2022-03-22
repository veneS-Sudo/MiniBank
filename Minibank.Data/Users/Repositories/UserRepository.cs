using System;
using System.Collections.Generic;
using System.Linq;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Data.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private static List<UserEntity> _userEntities = new();

        public User GetById(string id)
        {
            var entity = _userEntities.FirstOrDefault(user => user.Id.Equals(id));
            if (entity == null)
            {
                throw new ObjectNotFoundException($"Не существует пользователя c идентификатором {id}");
            }
                
            return new User
            {
                Id = entity.Id,
                Login = entity.Login,
                Email = entity.Email
            };
        }

        public List<User> GetAllUsers()
        {
            return
                _userEntities.Select(
                    userModel => new User
                    {
                        Id = userModel.Id,
                        Login = userModel.Login,
                        Email = userModel.Email
                    }).ToList();
        }

        public void CreateUser(User user)
        {
            var entity = new UserEntity
            {
                Id = Guid.NewGuid().ToString(),
                Login = user.Login,
                Email = user.Email
            };
            
            _userEntities.Add(entity);
        }

        public void UpdateUser(User user)
        {
            var entity = _userEntities.FirstOrDefault(userModel => userModel.Id == user.Id);
            if (entity == null)
            {
                throw new ObjectNotFoundException($"Не существует пользователя c идентификатором {user.Id}");
            }
            
            entity.Login = user.Login;
            entity.Email = entity.Email;
        }

        public void DeleteUser(string id)
        {
            var entity = _userEntities.FirstOrDefault(userModel => userModel.Id == id);
            if (entity == null)
            {
                throw new ObjectNotFoundException($"Не существует пользователя c идентификатором {id}");
            }
            
            _userEntities.Remove(entity);
        }

        public bool Exists(string id)
        {
            return _userEntities.Any(user => user.Id == id);
        }
    }
}