using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Transfers.Services
{
    public interface IMoneyTransferService
    {
        Task<string> TransferAmountAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken);
    }
}