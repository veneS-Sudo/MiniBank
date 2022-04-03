using System.Threading.Tasks;

namespace Minibank.Core.Converters
{
    public interface ICurrencyRateProvider
    {
        Task<double> GetCurrencyRateAsync(Currency currency);
    }
}