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

        public async Task<decimal> ConvertAsync(decimal amount, Currency fromCurrency, Currency toCurrency, CancellationToken cancellationToken)
        {
            if (amount < 0)
            {
                throw new ValidationException("Сумма перевода не может быть отрицательной!");
            }

            if (fromCurrency == toCurrency)
            {
                return amount;
            }

            var rateFromCurrency = await _currencyRateProvider.GetCurrencyRateAsync(fromCurrency, cancellationToken);
            var rateToCurrency = await _currencyRateProvider.GetCurrencyRateAsync(toCurrency, cancellationToken); 
            
            return amount * rateFromCurrency / rateToCurrency;
        }
    }
}