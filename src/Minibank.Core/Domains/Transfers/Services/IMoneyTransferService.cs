using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Transfers.Services
{
    public interface IMoneyTransferService
    {
        Task<List<MoneyTransfer>> GetAllTransfersAsync(string bankAccountId, CancellationToken cancellationToken);
        Task<string> TransferAmountAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken);
    }
}