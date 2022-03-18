namespace Minibank.Core.Converters
{
    public interface ICurrencyRateProvider
    {
        double GetCurrencyRate(Currency currency);
    }
}