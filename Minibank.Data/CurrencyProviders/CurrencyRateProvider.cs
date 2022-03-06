using System;
using System.Collections.Generic;
using Minibank.Core.Converters;
using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Data.CurrencyProviders
{
    public class CurrencyRateProvider : ICurrencyRateProvider
    {
        private readonly Dictionary<CodeCurrency, (int, int)> _currencyRates = new()
        {
            { CodeCurrency.Usd, (30, 124) },
            { CodeCurrency.Bp, (90, 150) },
            { CodeCurrency.Dk, (110, 160) },
            { CodeCurrency.Euro, (75, 136) },
            { CodeCurrency.Gm, (10, 500) }
        };

        private readonly Random _random;
        public CurrencyRateProvider()
        {
            _random = new Random();
        }
        
        public int GetCurrencyRate(CodeCurrency codeCurrency)
        {
            if (codeCurrency == CodeCurrency.Random)
                return _random.Next();

            if (!_currencyRates.TryGetValue(codeCurrency, out var currencyRate))
                throw new KeyNotFoundException($"Валюта c кодом: {codeCurrency}, не поддерживается.");

            return _random.Next(currencyRate.Item1, currencyRate.Item2);
        }
    }
}