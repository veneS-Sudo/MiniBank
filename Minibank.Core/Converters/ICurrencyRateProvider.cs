namespace Minibank.Core.Converters
{
    public interface ICurrencyRateProvider
    {
        int GetCurrencyRate(CodeCurrency codeCurrency);
    }
}