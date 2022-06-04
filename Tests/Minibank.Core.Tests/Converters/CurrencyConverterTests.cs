using System;
using System.Threading;
using Minibank.Core.Converters;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Converters
{
    public class CurrencyConverterTests
    {
        private readonly ICurrencyConverter _currencyConverter;
        private readonly Mock<ICurrencyRateProvider> _currencyRateProviderMock;
        public CurrencyConverterTests()
        {
            _currencyRateProviderMock = new Mock<ICurrencyRateProvider>();
            _currencyConverter = new CurrencyConverter(_currencyRateProviderMock.Object);
        }

        [Theory]
        [InlineData(Currency.RUB, Currency.EUR)]
        [InlineData(Currency.EUR, Currency.RUB)]
        [InlineData(Currency.USD, Currency.EUR)]
        [InlineData(Currency.USD, Currency.RUB)]
        public async void ConvertAsync_ConvertAmountFromOneToAnotherCurrency_ShouldReturnResultConversion(Currency from, Currency to)
        {
            var amount = Decimal.One;
            var expectedResultConversion = decimal.One;
            _currencyRateProviderMock.Setup(
                _ => _.GetCurrencyRateAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()).Result).Returns(
                    expectedResultConversion);

            var resultConversion = await _currencyConverter.ConvertAsync(amount, from, to, CancellationToken.None);
            
            Assert.Equal(expectedResultConversion, resultConversion, 2);
        }

        [Theory]
        [InlineData(Currency.RUB, Currency.RUB)]
        [InlineData(Currency.EUR, Currency.EUR)]
        [InlineData(Currency.USD, Currency.USD)]
        public async void ConvertAsync_ConvertAmountFromOneToSameCurrency_ShouldReturnSameAmount(Currency from, Currency to)
        {
            var amount = Decimal.One;

            var resultConversion = await _currencyConverter.ConvertAsync(amount, from, to, CancellationToken.None);
            
            Assert.Equal(amount, resultConversion);
        }

        [Fact]
        public async void ConvertAsync_ConvertAmountEqualZero_ShouldReturnZeroAsResult()
        {
            var amount = Decimal.Zero;
            var from = Currency.RUB;
            var to = Currency.USD;
            
            var resultConversion = await _currencyConverter.ConvertAsync(amount, from, to, CancellationToken.None);
            
            Assert.Equal(Decimal.Zero, resultConversion);
        }

        [Fact]
        public async void ConvertAsync_ConvertNegativeAmount_ThrowNegativeAmountException()
        {
            var amount = Decimal.MinusOne;
            var from = Currency.RUB;
            var to = Currency.USD;

            await Assert.ThrowsAsync<NegativeAmountConvertException>(
                () => _currencyConverter.ConvertAsync(amount, from, to, CancellationToken.None));
        }

        [Fact]
        public async void ConvertAsync_ConvertAmount_MethodGetCurrencyRateOfRateProviderShouldInvokeTwice()
        {
            var amount = Decimal.One;
            var from = Currency.RUB;
            var to = Currency.USD;
            var currencies = new[] { from, to };
            _currencyRateProviderMock.Setup(
                _ => _.GetCurrencyRateAsync(It.IsIn(currencies), It.IsAny<CancellationToken>()).Result).Returns(
                    Decimal.One);

            await _currencyConverter.ConvertAsync(amount, from, to, CancellationToken.None);

            _currencyRateProviderMock.Verify(
                _ => _.GetCurrencyRateAsync(It.IsIn(currencies), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
        
        [Fact]
        public async void ConvertAsync_ConvertAmountEqualZero_MethodGetCurrencyRateOfRateProviderShouldNeverInvoke()
        {
            var amount = Decimal.Zero;
            var from = Currency.RUB;
            var to = Currency.USD;

            await _currencyConverter.ConvertAsync(amount, from, to, CancellationToken.None);

            _currencyRateProviderMock.Verify(
                _ => _.GetCurrencyRateAsync(It.IsAny<Currency>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}