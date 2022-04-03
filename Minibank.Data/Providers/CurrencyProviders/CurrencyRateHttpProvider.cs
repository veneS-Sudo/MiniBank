using System.Net.Http;
using System.Net.Http.Json;
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
        
        public async Task<double> GetCurrencyRateAsync(Currency currency)
        {
            var response = await _httpClient.GetFromJsonAsync<CourseResponse>("daily_json.js");

            return response.Valute[currency.ToString()].Value;
        }
    }
}