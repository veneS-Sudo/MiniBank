using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id);
        Task<List<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task<bool> ExistsAsync(string id);
        Task DeleteUserAsync(string id);
    }
}