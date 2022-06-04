using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Converters;
using Minibank.Core.Exceptions.FriendlyExceptions;

namespace Minibank.Web.Controllers.Converters
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class CurrencyConverterController : ControllerBase
    {
        private readonly ICurrencyConverter _currencyConverter;

        public CurrencyConverterController(ICurrencyConverter currencyConverter)
        {
            _currencyConverter = currencyConverter;
        }

        [HttpGet("[action]")]
        public async Task<decimal> ConvertCurrency(decimal amount, string fromCurrency, string toCurrency, CancellationToken cancellationToken)
        {

            if (fromCurrency == null || !Enum.TryParse<Currency>(fromCurrency, true, out var from))
            {
                throw new ParametersValidationException("не удалось определить валюту, из которой необходимо конвертировать");
            }
            if (toCurrency == null || !Enum.TryParse<Currency>(toCurrency, true, out var to))
            {
                throw new ParametersValidationException("не удалось определить валюту, в которую необходимо конвертировать");
            }


            return Math.Round(await _currencyConverter.ConvertAsync(amount, from, to, cancellationToken), 2);
        }
    }
}