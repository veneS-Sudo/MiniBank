using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Repositories;
using Minibank.Core.Domains.Transfers.Services;
using Minibank.Core.Domains.Transfers.Validators;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Core.Tests.Domains.Transfers.EqualityComparers;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Domains.Transfers.Services
{
    public class MoneyTransferServiceTests
    {
        private readonly IMoneyTransferService _moneyTransferService;
        private readonly Mock<IMoneyTransferRepository> _moneyTransferRepositoryMock;
        private readonly Mock<ICurrencyConverter> _currencyConverterMock;
        private readonly Mock<ICommissionCalculator> _commissionCalculatorMock;
        private readonly Mock<IBankAccountRepository> _bankAccountRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public MoneyTransferServiceTests()
        {
            _moneyTransferRepositoryMock = new Mock<IMoneyTransferRepository>();
            _commissionCalculatorMock = new Mock<ICommissionCalculator>();
            _currencyConverterMock = new Mock<ICurrencyConverter>();
            _bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<MoneyTransferService>>();
            IValidator<MoneyTransfer> moneyTransferValidator = new MoneyTransferValidator(_bankAccountRepositoryMock.Object);

            _moneyTransferService = new MoneyTransferService(_moneyTransferRepositoryMock.Object,
                _currencyConverterMock.Object, moneyTransferValidator, _bankAccountRepositoryMock.Object,
                _unitOfWorkMock.Object, _commissionCalculatorMock.Object, loggerMock.Object);
        }

        private void SetupAssertions(out MoneyTransfer moneyTransfer)
        {
            var senderBankAccountId = "SomeAccountId_1";
            var recipientBankAccountId = "SomeAccountId_2";
            var senderBankAccount = new BankAccount() { Id = senderBankAccountId, Balance = 1000m, Currency = Currency.Eur };
            var recipientBankAccount = new BankAccount() { Id = recipientBankAccountId, Balance = 1000m, Currency = Currency.Eur };
            moneyTransfer = new MoneyTransfer()
            {
                Amount = decimal.One,
                Currency = Currency.Eur,
                FromBankAccountId = senderBankAccountId,
                ToBankAccountId = recipientBankAccountId
            };
            
            _bankAccountRepositoryMock.SetupExist(senderBankAccountId, recipientBankAccountId).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(senderBankAccountId, recipientBankAccountId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => 
                    _.GetByIdAsync(senderBankAccountId, It.IsAny<CancellationToken>()).Result).Returns(senderBankAccount);
            _bankAccountRepositoryMock.Setup(_ => 
                    _.GetByIdAsync(recipientBankAccountId, It.IsAny<CancellationToken>()).Result).Returns(recipientBankAccount);
            _bankAccountRepositoryMock.Setup(
                _ => _.UpdateAccountAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()).Result).Returns(true);
        }


        [Fact]
        public async void GetAllTransfersAsync_GetAllTransfersForExistBankAccount_ShouldReturnNotEmptyOrNullCollection()
        {
            var accountId = "SomeId";
            var moneyTransfers = new List<MoneyTransfer>() { new() };
            _moneyTransferRepositoryMock.Setup(
                _ => _.GetAllTransfersAsync(accountId, It.IsAny<CancellationToken>()).Result)
                .Returns(moneyTransfers);
            _bankAccountRepositoryMock.SetupExist(accountId).Returns(true);
            
            var actual = await _moneyTransferService.GetAllTransfersAsync(accountId, CancellationToken.None);
            
            Assert.NotNull(actual);
            Assert.NotEmpty(actual);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(5)]
        [InlineData(10)]
        [InlineData(42)]
        public async void GetAllTransfersAsync_GetAllTransfersForExistBankAccount_ShouldReturnSameCountItemsOfCollection(int count)
        {
            var accountId = "SomeId";
            var moneyTransfers = Enumerable.Repeat(new MoneyTransfer(), count).ToList();
            _moneyTransferRepositoryMock.Setup(
                _ => _.GetAllTransfersAsync(accountId, It.IsAny<CancellationToken>()).Result)
                .Returns(moneyTransfers);
            _bankAccountRepositoryMock.SetupExist(accountId).Returns(true);

            var actual = await _moneyTransferService.GetAllTransfersAsync(accountId, CancellationToken.None);

            Assert.Equal(count, actual.Count);
        }

        [Fact]
        public async void GetAllTransfersAsync_GetAllTransfersForExistBankAccount_ShouldReturnSameItems()
        {
            var accountId = "SomeId";
            var moneyTransfers = new List<MoneyTransfer>() 
                { new() {Id = "SomeId_1"}, new() {Id = "SomeId_2"}, new() {Id = "SomeId_3"} };
            _moneyTransferRepositoryMock.Setup(
                _ => _.GetAllTransfersAsync(accountId, It.IsAny<CancellationToken>()).Result)
                .Returns(moneyTransfers);
            _bankAccountRepositoryMock.SetupExist(accountId).Returns(true);
            
            var actual = await _moneyTransferService.GetAllTransfersAsync(accountId, CancellationToken.None);
            
            Assert.Equal(moneyTransfers, actual, new MoneyTransferEqualityComparer());
        }

        [Fact]
        public async void GetAllTransfersAsync_GetAllTransfersForNotExistBankAccount_ShouldThrowObjectNotFoundException()
        {
            var accountId = "SomeId";
            _bankAccountRepositoryMock.SetupExist(accountId).Returns(false);

            await Assert.ThrowsAsync<ObjectNotFoundException>(
                () => _moneyTransferService.GetAllTransfersAsync(accountId, CancellationToken.None));
        }

        [Fact]
        public async void GetAllTransfersAsync_GetAllTransfersForExistBankAccount_MethodGetAllOfTransferRepositoryShouldInvokeOnce()
        {
            var accountId = "SomeId";
            _bankAccountRepositoryMock.SetupExist(accountId).Returns(true);
            
            await _moneyTransferService.GetAllTransfersAsync(accountId, CancellationToken.None);

            _moneyTransferRepositoryMock.Verify(_ => _.GetAllTransfersAsync(accountId, It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferPositiveAmountBetweenExistAccounts_ShouldReturnTransferId()
        {
            SetupAssertions(out var moneyTransfer);
            _moneyTransferRepositoryMock.Setup(
                _ => _.CreateTransferAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()).Result).Returns(
                    "SomeId");

            var transferId = await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            Assert.NotEmpty(transferId);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100000)]
        public async void TransferAmountAsync_TransferAmountEqualOrLeastZero_ShouldThrowValidationExceptionException(decimal amount)
        {
            var moneyTransfer = new MoneyTransfer() { Amount = amount };

            await Assert.ThrowsAsync<ValidationException>(
                () => _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmountBetweenSameAccount_ThrowValidationException()
        {
            var bankAccountId = "SomeBankAccountId";
            var moneyTransfer = new MoneyTransfer()
            {
                Amount = Decimal.One,
                FromBankAccountId = bankAccountId,
                ToBankAccountId = bankAccountId
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodGetByIdOfAccountRepositoryShouldInvokeTwice()
        {
            SetupAssertions(out var moneyTransfer);

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(
                _ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.AtLeast(2));
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodGetByIdOfAccountRepositoryShouldInvokeWithSameSenderId()
        {
            SetupAssertions(out var moneyTransfer);
            var id = moneyTransfer.FromBankAccountId;

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(
                _ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()));
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodGetByIdOfAccountRepositoryShouldInvokeWithSameRecipientId()
        {
            SetupAssertions(out var moneyTransfer);
            var id = moneyTransfer.ToBankAccountId;

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(
                _ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmountFromAccountWithInsufficientAmount_ShouldThrowLackOfFundsException()
        {
            var bankAccounts = new[] { "SomeAccountId_1", "SomeAccountId_2" };
            var moneyTransfer = new MoneyTransfer()
            {
                Amount = decimal.One,
                Currency = Currency.Eur,
                FromBankAccountId = bankAccounts[0],
                ToBankAccountId = bankAccounts[1]
            };
            var bankAccount = new BankAccount() { Balance = decimal.Zero };
            _bankAccountRepositoryMock.SetupExist(bankAccounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(bankAccounts).Returns(true);
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(It.IsIn(bankAccounts), It.IsAny<CancellationToken>()).Result).Returns(bankAccount);

            await Assert.ThrowsAsync<LackOfFundsException>(
                () => _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodUpdateAccountOfBankAccountRepositoryShouldInvokeTwice()
        {
            SetupAssertions(out var moneyTransfer);

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(
                _ => _.UpdateAccountAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodUpdateAccountOfBankAccountRepositoryShouldInvokeWithSameSenderId()
        {
            SetupAssertions(out var moneyTransfer);
            var senderId = moneyTransfer.FromBankAccountId; 
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(
                _ => _.UpdateAccountAsync(It.Is<BankAccount>(account => account.Id == senderId),
                        It.IsAny<CancellationToken>()));
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodUpdateAccountOfBankAccountRepositoryShouldInvokeWithSameRecipientId()
        {
            SetupAssertions(out var moneyTransfer);
            var recipientId = moneyTransfer.ToBankAccountId; 
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(
                _ => _.UpdateAccountAsync(It.Is<BankAccount>(account => account.Id == recipientId),
                        It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmountButSenderNotUpdated_ThrowTransferNotCompletedException()
        {
            SetupAssertions(out var moneyTransfer);
            var senderId = moneyTransfer.FromBankAccountId;
            _bankAccountRepositoryMock.Setup(
                _ => _.UpdateAccountAsync(It.Is<BankAccount>(account => account.Id == senderId),
                        It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await Assert.ThrowsAsync<TransferNotCompletedException>(
                () => _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None));
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmountButRecipientNotUpdated_ThrowTransferNotCompletedException()
        {
            SetupAssertions(out var moneyTransfer);
            var recipientId = moneyTransfer.ToBankAccountId;
            _bankAccountRepositoryMock.Setup(
                _ => _.UpdateAccountAsync(It.Is<BankAccount>(account => account.Id == recipientId),
                    It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await Assert.ThrowsAsync<TransferNotCompletedException>(
                () => _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None));    
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmountButSenderAndRecipientNotUpdated_ThrowTransferNotCompletedException()
        {
            SetupAssertions(out var moneyTransfer);
            var senderId = moneyTransfer.FromBankAccountId;
            var recipientId = moneyTransfer.ToBankAccountId;
            _bankAccountRepositoryMock.Setup(
                _ => _.UpdateAccountAsync(It.Is<BankAccount>(account => account.Id == senderId),
                    It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            _bankAccountRepositoryMock.Setup(
                _ => _.UpdateAccountAsync(It.Is<BankAccount>(account => account.Id == recipientId),
                    It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await Assert.ThrowsAsync<TransferNotCompletedException>(
                () => _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodCreateTransferOfMoneyTransferRepositoryShouldInvokeOnce()
        {
            SetupAssertions(out var moneyTransfer);

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _moneyTransferRepositoryMock.Verify(
                _ => _.CreateTransferAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodCreateTransferOfMoneyTransferRepositoryShouldInvokeWithSameMoneyTransfer()
        {
            SetupAssertions(out var moneyTransfer);

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _moneyTransferRepositoryMock.Verify(
                _ => _.CreateTransferAsync(It.Is(moneyTransfer, new MoneyTransferWithoutIdEqualityComparer()),
                        It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodSaveChangesOfUnitOfWorkShouldInvokeOnce()
        {
            SetupAssertions(out var moneyTransfer);

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _unitOfWorkMock.Verify(
                _ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodSaveOfUnitOfWorkShouldInvokeOnceAfterAccountsUpdateAndTransferCreate()
        {
            var lastInvocation = string.Empty;
            var expectedInvocation = "saveChanges";
            SetupAssertions(out var moneyTransfer);
            _bankAccountRepositoryMock.Setup(
                _ => _.UpdateAccountAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()).Result).Returns(true)
                .Callback(() => lastInvocation = "update");
            _moneyTransferRepositoryMock.Setup(
                _ => _.CreateTransferAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()).Result)
                .Callback(() => lastInvocation = "createMoneyTransfer");
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result)
                .Callback(() => lastInvocation = expectedInvocation);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            Assert.Equal(expectedInvocation, lastInvocation);
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmountBetweenAccountsWithDifferentUserId_CommissionShouldDeductedFromAmount()
        {
            var bankAccounts = new[] { "SomeAccountId_1", "SomeAccountId_2" };
            var usersId = new[] { "SomeUserId_1", "SomeUserId_2" };
            var moneyTransfer = new MoneyTransfer()
            {
                Amount = 100m,
                Currency = Currency.Eur,
                FromBankAccountId = bankAccounts[0],
                ToBankAccountId = bankAccounts[1]
            };
            var commission = moneyTransfer.Amount * 0.15m;
            var resultAmountTransfer = moneyTransfer.Amount - commission;
            var bankAccountSender = new BankAccount() { Balance = 100m, UserId = usersId[0] };
            var bankAccountRecipient = new BankAccount() { Balance = 100m, UserId = usersId[1] };
            _bankAccountRepositoryMock.SetupExist(bankAccounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(bankAccounts).Returns(true);
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(bankAccounts[0], It.IsAny<CancellationToken>()).Result).Returns(bankAccountSender);
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(bankAccounts[1], It.IsAny<CancellationToken>()).Result).Returns(bankAccountRecipient);
            _commissionCalculatorMock.Setup(
                _ => _.CalculateCommissionAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()).Result)
                .Returns(commission);
            _bankAccountRepositoryMock.Setup(
                _ => _.UpdateAccountAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()).Result).Returns(true);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);
            
            Assert.Equal(resultAmountTransfer, moneyTransfer.Amount);
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodConvertOfCurrencyConverterShouldInvokeOnce()
        {
           SetupAssertions(out var moneyTransfer);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _currencyConverterMock.Verify(_ =>
                    _.ConvertAsync(It.IsAny<decimal>(), It.IsAny<Currency>(), It.IsAny<Currency>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodConvertOfCurrencyConverterShouldInvokeWithSameFromCurrency()
        {
           SetupAssertions(out var moneyTransfer);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _currencyConverterMock.Verify(_ =>
                    _.ConvertAsync(It.IsAny<decimal>(), moneyTransfer.Currency, It.IsAny<Currency>(),
                        It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodCalculateCommissionOfCommissionCalculatorShouldInvokeOnce()
        {
            SetupAssertions(out var moneyTransfer);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _commissionCalculatorMock.Verify(_ => 
                _.CalculateCommissionAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodCalculateCommissionOfCommissionCalculatorShouldInvokeWithSameMoneyTransfer()
        {
            SetupAssertions(out var moneyTransfer);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _commissionCalculatorMock.Verify(_ =>
                _.CalculateCommissionAsync(It.Is(moneyTransfer, new MoneyTransferWithoutIdEqualityComparer()),
                    It.IsAny<CancellationToken>()));
        }
    }
}