using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
        Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken);
        Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken);
    }
}