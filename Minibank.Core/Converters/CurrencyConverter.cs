using System;
using System.Threading.Tasks;
using Minibank.Core.Exceptions.FriendlyExceptions;

namespace Minibank.Core.Converters
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;

        public CurrencyConverter(ICurrencyRateProvider currencyRateProvider) => 
            _currencyRateProvider = currencyRateProvider;

        public async Task<double> ConvertAsync(double amount, Currency fromCurrency, Currency toCurrency)
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
                return amount * await _currencyRateProvider.GetCurrencyRateAsync(fromCurrency);
            }
            if (fromCurrency == Currency.RUB)
            {
                return amount / await _currencyRateProvider.GetCurrencyRateAsync(toCurrency);
            }
            
            return amount * await _currencyRateProvider.GetCurrencyRateAsync(fromCurrency) /
                   await _currencyRateProvider.GetCurrencyRateAsync(toCurrency);
        }
    }
}