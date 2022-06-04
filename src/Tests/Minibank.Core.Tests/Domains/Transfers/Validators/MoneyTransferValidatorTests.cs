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
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2"; 
            var transfer = new MoneyTransfer()
            {
                Amount = 10,
                Currency = Currency.RUB,
                FromBankAccountId = senderBankAccountId,
                ToBankAccountId = recipientBankAccountId
            };
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId, recipientBankAccountId).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(senderBankAccountId, recipientBankAccountId).Returns(true);

            var validationResult = await _validator.TestValidateAsync(transfer);
            
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async void ValidateAsync_ValidateTransfer_MethodIsExistOfBankAccountRepositoryShouldInvokeTwice()
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var transfer = new MoneyTransfer() { FromBankAccountId = senderBankAccountId, ToBankAccountId = recipientBankAccountId };
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId, recipientBankAccountId).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(senderBankAccountId, recipientBankAccountId).Returns(true);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.Verify(_ => 
                    _.IsExistAsync(It.IsIn(senderBankAccountId, recipientBankAccountId), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotExistAccounts_MethodIsExistOfBankAccountRepositoryShouldInvokeOnceOnlyForFirstAccount()
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var transfer = new MoneyTransfer() { FromBankAccountId = senderBankAccountId, ToBankAccountId = recipientBankAccountId };
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId, recipientBankAccountId).Returns(false);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.VerifyExist(senderBankAccountId, Times.Once);
            _bankAccountRepositoryMock.VerifyExist(recipientBankAccountId, Times.Never);
        }

        [Fact]
        public async void ValidateAsync_ValidateTransfer_MethodIsOpenOfBankAccountRepositoryShouldInvokeOnce()
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var transfer = new MoneyTransfer() { FromBankAccountId = senderBankAccountId, ToBankAccountId = recipientBankAccountId };
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId, recipientBankAccountId).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(senderBankAccountId, recipientBankAccountId).Returns(true);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.Verify(_ => 
                    _.IsOpenAsync(It.IsIn(senderBankAccountId, recipientBankAccountId), It.IsAny<CancellationToken>()),
                Times.Exactly(2));
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotOpennessAccount_MethodIsOpenOfBankAccountRepositoryShouldInvokeOnceOnlyForFirstAccount()
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var transfer = new MoneyTransfer() { FromBankAccountId = senderBankAccountId, ToBankAccountId = recipientBankAccountId };
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId, recipientBankAccountId).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(senderBankAccountId, recipientBankAccountId).Returns(false);

            await _validator.TestValidateAsync(transfer);
            
            _bankAccountRepositoryMock.VerifyOpenness(senderBankAccountId, Times.Once);
            _bankAccountRepositoryMock.VerifyOpenness(recipientBankAccountId, Times.Never);
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotExistAccounts_MethodIsOpenOfBankAccountRepositoryShouldNeverInvoke()
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var transfer = new MoneyTransfer() { FromBankAccountId = senderBankAccountId, ToBankAccountId = recipientBankAccountId };
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId, recipientBankAccountId).Returns(false);

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.Verify(_ => 
                    _.IsOpenAsync(It.IsIn(senderBankAccountId, recipientBankAccountId), It.IsAny<CancellationToken>()),
                Times.Never);    
        }
        
        [Fact]
        public async void ValidateAsync_ValidateTransferWithNotExistSecondAccount_MethodIsOpenOfBankAccountRepositoryShouldNeverInvokeForSecondAccount()
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var transfer = new MoneyTransfer() { FromBankAccountId = senderBankAccountId, ToBankAccountId = recipientBankAccountId };
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(senderBankAccountId).Returns(true);
            _bankAccountRepositoryMock.SetupExist(recipientBankAccountId).Returns(false);
            

            await _validator.TestValidateAsync(transfer);

            _bankAccountRepositoryMock.VerifyOpenness(recipientBankAccountId, Times.Never);
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
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = senderBankAccountId;
            var moneyTransfer = new MoneyTransfer() { FromBankAccountId = senderBankAccountId, ToBankAccountId = recipientBankAccountId };

            var validationResult = await _validator.TestValidateAsync(moneyTransfer);

            validationResult.ShouldHaveValidationErrorFor(transfer => transfer);
        }

        [Fact]
        public async void ValidateAsync_ValidateTransferWithIncorrectCurrencyCode_MemberCurrencyShouldInvalid()
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var incorrectCurrency = Enum.GetValues<Currency>().Max() + 1;
            var moneyTransfer = new MoneyTransfer()
            {
                FromBankAccountId = senderBankAccountId,
                ToBankAccountId = recipientBankAccountId,
                Currency = incorrectCurrency
            };

            var validationResult = await _validator.TestValidateAsync(moneyTransfer);

            validationResult.ShouldHaveValidationErrorFor(transfer => transfer.Currency);
        }
    }
}