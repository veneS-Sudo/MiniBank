using System.Collections.Generic;
using System.Threading.Tasks;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Accounts.Services
{
    public interface IBankAccountService
    {
        Task<BankAccount> GetByIdAsync(string id);
        Task<List<BankAccount>> GetAllAccountsAsync();
        Task CreateAccountAsync(BankAccount bankAccount);
        Task UpdateAccountAsync(BankAccount bankAccount);
        Task CloseAccountAsync(string id);
        Task<double> CalculateCommissionAsync(MoneyTransfer moneyTransfer);
        Task TransferAmountAsync(MoneyTransfer moneyTransfer);
    }
}