using System;
using System.Linq;
using System.Threading;
using FluentValidation;
using FluentValidation.TestHelper;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Validators;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Domains.Transfers.Validators
{
    public class MoneyTransferValidatorTests
    {
        private readonly IValidator<MoneyTransfer> _validator;
        private readonly Mock<IBankAccountRepository> _bankAccountRepositoryMock;

        public MoneyTransferValidatorTests()
        {
            _bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            _validator = new MoneyTransferValidator(_bankAccountRepositoryMock.Object);
        }

        [Fact]
        public async void ValidateAsync_SuccessPath_MoneyTransferShouldValid()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_2" };
            var transfer = new MoneyTransfer()
            {
                Amount = 10,
                Currency = Currency.RUB,
                FromBankAccountId = accounts[0],
                ToBankAccountId = accounts[1]
            };
            _bankAccountRepositoryMock.SetupExist(accounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(accounts).Returns(true);

            var validationResult = await _validator.TestValidateAsync(transfer);
            
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async void ValidateAsync_ValidateTransfer_MethodIsExistOfBankAccountRepositoryShouldInvokeTwice()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_2" };
            var transfer = new MoneyTransfer() { FromBankAccountId = accounts[0], ToBankAccountId = accounts[1] };
            _bankAccountRepositoryMock.SetupExist(accounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(accounts).Returns(true);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.Verify(_ => _.IsExistAsync(It.IsIn(accounts), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotExistAccounts_MethodIsExistOfBankAccountRepositoryShouldInvokeOnceOnlyForFirstAccount()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_2" };
            var transfer = new MoneyTransfer() { FromBankAccountId = accounts[0], ToBankAccountId = accounts[1] };
            _bankAccountRepositoryMock.SetupExist(accounts).Returns(false);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.VerifyExist(accounts[0], Times.Once);
            _bankAccountRepositoryMock.VerifyExist(accounts[1], Times.Never);
        }

        [Fact]
        public async void ValidateAsync_ValidateTransfer_MethodIsOpenOfBankAccountRepositoryShouldInvokeOnce()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_2" };
            var transfer = new MoneyTransfer() { FromBankAccountId = accounts[0], ToBankAccountId = accounts[1] };
            _bankAccountRepositoryMock.SetupExist(accounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(accounts).Returns(true);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.Verify(_ => _.IsOpenAsync(It.IsIn(accounts), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotOpennessAccount_MethodIsOpenOfBankAccountRepositoryShouldInvokeOnceOnlyForFirstAccount()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_2" };
            var transfer = new MoneyTransfer() { FromBankAccountId = accounts[0], ToBankAccountId = accounts[1] };
            _bankAccountRepositoryMock.SetupExist(accounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(accounts).Returns(false);

            await _validator.TestValidateAsync(transfer);
            
            _bankAccountRepositoryMock.VerifyOpenness(accounts[0], Times.Once);
            _bankAccountRepositoryMock.VerifyOpenness(accounts[1], Times.Never);
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotExistAccounts_MethodIsOpenOfBankAccountRepositoryShouldNeverInvoke()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_2" };
            var transfer = new MoneyTransfer() { FromBankAccountId = accounts[0], ToBankAccountId = accounts[1] };
            _bankAccountRepositoryMock.SetupExist(accounts).Returns(false);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.Verify(_ => _.IsOpenAsync(It.IsIn(accounts), It.IsAny<CancellationToken>()),
                Times.Never);    
        }
        
        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotExistSecondAccount_MethodIsOpenOfBankAccountRepositoryShouldNeverInvokeForSecondAccount()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_2" };
            var transfer = new MoneyTransfer() { FromBankAccountId = accounts[0], ToBankAccountId = accounts[1] };
            _bankAccountRepositoryMock.SetupExist(accounts[0]).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(accounts[0]).Returns(true);
            _bankAccountRepositoryMock.SetupExist(accounts[1]).Returns(false);
            

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.VerifyOpenness(accounts[1], Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.01)]
        [InlineData(-100000)]
        public async void ValidateAsync_ValidateTransferWithNotPositiveAmount_MemberAmountShouldInvalid(decimal amount)
        {
            var moneyTransfer = new MoneyTransfer() { Amount = amount };

            var validationResult = await _validator.TestValidateAsync(moneyTransfer);

            validationResult.ShouldHaveValidationErrorFor(transfer => transfer.Amount);
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithSameBankAccountId_TransferShouldInvalid()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_1" };
            var moneyTransfer = new MoneyTransfer() { FromBankAccountId = accounts[0], ToBankAccountId = accounts[1] };

            var validationResult = await _validator.TestValidateAsync(moneyTransfer);

            validationResult.ShouldHaveValidationErrorFor(transfer => transfer);
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithIncorrectCurrencyCode_MemberCurrencyShouldInvalid()
        {
            var accounts = new[] { "SomeAccount_1", "SomeAccount_1" };
            var incorrectCurrency = Enum.GetValues<Currency>().Max() + 1;
            var moneyTransfer = new MoneyTransfer()
            {
                FromBankAccountId = accounts[0],
                ToBankAccountId = accounts[1],
                Currency = incorrectCurrency
            };

            var validationResult = await _validator.TestValidateAsync(moneyTransfer);

            validationResult.ShouldHaveValidationErrorFor(transfer => transfer.Currency);
        }
    }
}