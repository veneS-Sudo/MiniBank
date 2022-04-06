﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Users.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken);
        Task CreateUserAsync(User user, CancellationToken cancellationToken);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(string id, CancellationToken cancellationToken);
        Task DeleteUserAsync(string id, CancellationToken cancellationToken);
    }
}