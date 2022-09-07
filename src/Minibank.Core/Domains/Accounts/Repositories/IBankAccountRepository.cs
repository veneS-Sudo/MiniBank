using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Domains.Interfaces;

namespace Minibank.Core.Domains.Accounts.Repositories
{
    public interface IBankAccountRepository : IEntityExistence<string>
    {
        Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken);
        Task<BankAccount> CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task<bool> UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task<bool> ExistByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task<bool> CloseAccountAsync(string id, CancellationToken cancellationToken);
        Task<bool> IsOpenAsync(string id, CancellationToken cancellationToken);
    }
}