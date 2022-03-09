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
            if (sumConvert < 0) 
                throw new UserFriendlyException("Сумма первода не может быть отрицательной!");
            
            long resultConversion;
            checked
            {
                resultConversion = (long)sumConvert * _currencyRateProvider.GetCurrencyRate(targetCodeCurrency);
            }
            
            return resultConversion;
        }
    }
}