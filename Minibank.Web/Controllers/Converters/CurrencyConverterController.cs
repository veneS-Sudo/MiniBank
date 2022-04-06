using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Converters;

namespace Minibank.Web.Controllers.Converters
{
    [ApiController]
    [Route("[controller]")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _currencyConverter;

        public CurrencyConverterController(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

        [HttpGet]
        public async Task<double> Get(int amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken)
        {
            return Math.Round( 
                await _currencyConverter.ConvertAsync(amount,
                    Enum.Parse<Currency>(fromCurrency, true),
                    Enum.Parse<Currency>(toCurrency, true),
                    cancellationToken),
                2);
        }
    }
}