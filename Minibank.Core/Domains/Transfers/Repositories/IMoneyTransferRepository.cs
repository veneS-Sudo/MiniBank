using System.Collections.Generic;
using System.Threading.Tasks;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Transfers.Repositories
{
    public interface IMoneyTransferRepository
    {
        Task<MoneyTransfer> GetByIdAsync(string id);
        Task<List<MoneyTransfer>> GetAllTransfersAsync();
        Task CreateTransferAsync(double amount, string fromAccountId, string toAccountId, Currency currency);
        Task CreateTransferAsync(MoneyTransfer moneyTransfer);
    }
}