namespace Minibank.Core.Converters
{
    public interface ICurrencyConverter
    {
        double Convert(double amount, Currency fromCurrency, Currency toCurrency);
    }
}