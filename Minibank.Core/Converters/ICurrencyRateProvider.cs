using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Converters
{
    public interface ICurrencyRateProvider
    {
        Task<decimal> GetCurrencyRateAsync(Currency currency, CancellationToken cancellationToken);
    }
}