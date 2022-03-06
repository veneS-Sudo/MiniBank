using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Core.Converters
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly ICurrencyRateProvider _currencyRateProvider;

        public CurrencyConverter(ICurrencyRateProvider currencyRateProvider) => 
            _currencyRateProvider = currencyRateProvider;

        public long Convert(int sumConvert, CodeCurrency targetCodeCurrency)
        {
            long resultConversion;
            checked
            {
                resultConversion = (long)sumConvert * _currencyRateProvider.GetCurrencyRate(targetCodeCurrency);
            }

            if (resultConversion < 0) 
                throw new UserFriendlyException("В результате конвертации получено отрицательное число.");
            return resultConversion;
        }
    }
}