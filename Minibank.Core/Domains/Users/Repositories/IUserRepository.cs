using System.Collections.Generic;

namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        User GetById(string id);
        List<User> GetAllUsers();
        void CreateUser(User user);
        void UpdateUser(User user);
        void DeleteUser(string id);
        bool Exists(string id);
    }
}