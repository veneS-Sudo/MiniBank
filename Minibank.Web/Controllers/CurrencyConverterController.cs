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
        public long Get(int amount, string targetCurrency)
        {
            if (!Enum.TryParse<CodeCurrency>(targetCurrency, true, out var codeCurrency) ||
                !Enum.IsDefined(codeCurrency))
                codeCurrency = CodeCurrency.Random;

            return _currencyConverter.Convert(amount, codeCurrency);
        }
    }
}