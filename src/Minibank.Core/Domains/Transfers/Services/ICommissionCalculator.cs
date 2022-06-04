using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Transfers.Services
{
    public interface ICommissionCalculator
    {
        Task<decimal> CalculateCommissionAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken);
    }
}