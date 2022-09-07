using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Converters;
using Minibank.Data.HttpClients.Models;

namespace Minibank.Data.Providers.CurrencyProviders
{
    public class CurrencyRateHttpProvider : ICurrencyRateProvider
    {
        private readonly HttpClient _httpClient; 
        
        public CurrencyRateHttpProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public async Task<decimal> GetCurrencyRateAsync(Currency currency, CancellationToken cancellationToken)
        {
            if (currency == Currency.Rub)
            {
                return 1;
            }
            
            var response = await _httpClient.GetFromJsonAsync<CourseResponse>("daily_json.js", cancellationToken);
    
            return response.Valute[currency.ToString()].Value;
        }
    }
}