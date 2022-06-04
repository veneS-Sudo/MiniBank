using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Converters
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertAsync(decimal amount, Currency fromCurrency, Currency toCurrency, CancellationToken cancellationToken);
    }
}