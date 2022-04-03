using System.Collections.Generic;
using System.Threading.Tasks;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Accounts.Repositories
{
    public interface IBankAccountRepository
    {
        Task<BankAccount> GetByIdAsync(string id);
        Task<List<BankAccount>> GetAllAccountsAsync();
        Task CreateAccountAsync(BankAccount bankAccount);
        Task UpdateAccountAsync(BankAccount bankAccount);
        Task<bool> ExistsByUserIdAsync(string userId);
        Task CloseAccountAsync(string id);
        Task<bool> BankAccountIsOpenAsync(string id);
    }
}