using System;
using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Core.Converters
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;

        public CurrencyConverter(ICurrencyRateProvider currencyRateProvider) => 
            _currencyRateProvider = currencyRateProvider;

        public double Convert(double amount, Currency fromCurrency, Currency toCurrency)
        {
            if (amount < 0)
            {
                throw new ValidationException("Сумма первода не может быть отрицательной!");
            }

            // TODO Change algorithm
            if (toCurrency == Currency.RUB)
                return amount * _currencyRateProvider.GetCurrencyRate(fromCurrency);
            if (fromCurrency == Currency.RUB)
                return amount / _currencyRateProvider.GetCurrencyRate(toCurrency);
            return amount * _currencyRateProvider.GetCurrencyRate(fromCurrency) /
                   _currencyRateProvider.GetCurrencyRate(toCurrency);
        }
    }
}