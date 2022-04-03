using System.Collections.Generic;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Users.Services
{
    public interface IUserService
    {
        Task<User> GetByIdAsync(string id);
        Task<List<User>> GetAllUsersAsync();
        Task CreateUserAsync(User user);
        Task UpdateUserAsync(User user);
        Task DeleteUserAsync(string id);
    }
}