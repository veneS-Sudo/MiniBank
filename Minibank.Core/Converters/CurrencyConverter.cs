using System;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Exceptions.FriendlyExceptions;

namespace Minibank.Core.Converters
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;

        public CurrencyConverter(ICurrencyRateProvider currencyRateProvider)
        {
            _currencyRateProvider = currencyRateProvider;    
        }

        public async Task<double> ConvertAsync(double amount, Currency fromCurrency, Currency toCurrency, CancellationToken cancellationToken)
        {
            if (amount < 0)
            {
                throw new ValidationException("Сумма первода не может быть отрицательной!");
            }

            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            // TODO Change algorithm
            if (toCurrency == Currency.RUB)
            {
                return amount * await _currencyRateProvider.GetCurrencyRateAsync(fromCurrency, cancellationToken);
            }
            if (fromCurrency == Currency.RUB)
            {
                return amount / await _currencyRateProvider.GetCurrencyRateAsync(toCurrency, cancellationToken);
            }
            
            return amount * await _currencyRateProvider.GetCurrencyRateAsync(fromCurrency, cancellationToken) /
                   await _currencyRateProvider.GetCurrencyRateAsync(toCurrency, cancellationToken);
        }
    }
}