using System;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Converters;

namespace Minibank.Web.Controllers
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
        public double Get(int amount, string fromCurrency, string toCurrency)
        {
            return Math.Round( 
                _currencyConverter.Convert(amount,
                    Enum.Parse<Currency>(fromCurrency),
                    Enum.Parse<Currency>(toCurrency, true)),
                2);
        }
    }
}