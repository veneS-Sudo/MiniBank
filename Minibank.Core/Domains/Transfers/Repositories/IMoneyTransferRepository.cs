using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Transfers.Repositories
{
    public interface IMoneyTransferRepository
    {
        Task<MoneyTransfer> GetByIdAsync(string id, CancellationToken cancellationToken);
        Task<List<MoneyTransfer>> GetAllTransfersAsync(CancellationToken cancellationToken);
        Task<MoneyTransfer> CreateTransferAsync(decimal amount, string fromAccountId, string toAccountId, Currency currency, CancellationToken cancellationToken);
        Task<MoneyTransfer> CreateTransferAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken);
    }
}