using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Minibank.Core.Converters;
using System.Text.Json;
using Minibank.Core.Exceptions.FriendlyException;
using Minibank.Data.HttpClients.Models;

namespace Minibank.Data.CurrencyProviders
{
    public class CurrencyRateHttpProvider : ICurrencyRateProvider
    {
        private readonly HttpClient _httpClient; 
        
        public CurrencyRateHttpProvider(IHttpClientFactory clientFactory, HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        
        public double GetCurrencyRate(Currency currency)
        {
            var response = _httpClient.GetFromJsonAsync<CourseResponse>("daily_json.js")
                .GetAwaiter().GetResult();
            
            return response.Valute[currency.ToString()].Value;
        }
    }
}