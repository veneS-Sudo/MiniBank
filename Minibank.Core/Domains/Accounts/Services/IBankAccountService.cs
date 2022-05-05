using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Accounts.Services
{
    public interface IBankAccountService
    {
        Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken);
        Task<BankAccount> CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task<BankAccount> UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken);
        Task<bool> CloseAccountAsync(string id, CancellationToken cancellationToken);
        Task<decimal> CalculateCommissionAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken);
        Task<MoneyTransfer> TransferAmountAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken);
    }
}