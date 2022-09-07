using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Accounts.Services
{
    public interface IBankAccountService
    {
        Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken);
        Task<BankAccount> CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task<bool> UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task<bool> CloseAccountAsync(string id, CancellationToken cancellationToken);
    }
}