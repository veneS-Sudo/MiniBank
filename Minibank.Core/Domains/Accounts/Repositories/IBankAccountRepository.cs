using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Accounts.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken);
        Task CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken);
        Task CloseAccountAsync(string id, CancellationToken cancellationToken);
        Task<bool> BankAccountIsOpenAsync(string id, CancellationToken cancellationToken);
    }
}