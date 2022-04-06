using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Converters
{
    public interface ICurrencyConverter
    {
        Task<double> ConvertAsync(double amount, Currency fromCurrency, Currency toCurrency, CancellationToken cancellationToken);
    }
}