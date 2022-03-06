namespace Minibank.Core.Converters
{
    public interface ICurrencyConverter
    {
        long Convert(int sumConvert, CodeCurrency targetCodeCurrency);
    }
}