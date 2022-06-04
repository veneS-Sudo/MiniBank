using System;
using System.Threading;
using FluentValidation;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Repositories;
using Minibank.Core.Domains.Transfers.Services;
using Minibank.Core.Domains.Transfers.Validators;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Moq;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

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
            IValidator<MoneyTransfer> moneyTransferValidator = new MoneyTransferValidator(_bankAccountRepositoryMock.Object);

            _moneyTransferService = new MoneyTransferService(_moneyTransferRepositoryMock.Object,
                _currencyConverterMock.Object, moneyTransferValidator, _bankAccountRepositoryMock.Object,
                _unitOfWorkMock.Object, _commissionCalculatorMock.Object);
        }

        private void SetupAssertions(out MoneyTransfer moneyTransfer)
        {
            var bankAccounts = new[] { "SomeAccountId_1", "SomeAccountId_2" };
            moneyTransfer = new MoneyTransfer()
            {
                Amount = decimal.One,
                Currency = Currency.EUR,
                FromBankAccountId = bankAccounts[0],
                ToBankAccountId = bankAccounts[1]
            };
            var bankAccount = new BankAccount() { Balance = 1000m };
            _bankAccountRepositoryMock.SetupExist(bankAccounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(bankAccounts).Returns(true);
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(It.IsIn(bankAccounts), It.IsAny<CancellationToken>()).Result).Returns(bankAccount);
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
                _ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmountFromAccountWithInsufficientAmount_ShouldThrowLackOfFundsException()
        {
            var bankAccounts = new[] { "SomeAccountId_1", "SomeAccountId_2" };
            var moneyTransfer = new MoneyTransfer()
            {
                Amount = decimal.One,
                Currency = Currency.EUR,
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
        public async void TransferAmountAsync_TransferAmount_MethodCreateTransferOfMoneyTransferRepositoryShouldInvokeOnce()
        {
            SetupAssertions(out var moneyTransfer);

            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _moneyTransferRepositoryMock.Verify(
                _ => _.CreateTransferAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()), Times.Once);
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
                _ => _.UpdateAccountAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()).Result).Callback(
                    () => lastInvocation = "update");
            _moneyTransferRepositoryMock.Setup(
                _ => _.CreateTransferAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()).Result).Callback(
                    () => lastInvocation = "createMoneyTransfer");
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Callback(
                () => lastInvocation = expectedInvocation);
            
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
                Currency = Currency.EUR,
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
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);
            
            Assert.Equal(resultAmountTransfer, moneyTransfer.Amount);
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodConvertOfCurrencyConverterShouldInvokeOnce()
        {
           SetupAssertions(out var moneyTransfer);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _currencyConverterMock.Verify(
                _ =>
                    _.ConvertAsync(It.IsAny<decimal>(), It.IsAny<Currency>(), It.IsAny<Currency>(),
                        It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void TransferAmountAsync_TransferAmount_MethodCalculateCommissionOfCommissionCalculatorShouldInvokeOnce()
        {
            SetupAssertions(out var moneyTransfer);
            
            await _moneyTransferService.TransferAmountAsync(moneyTransfer, CancellationToken.None);

            _commissionCalculatorMock.Verify(
                _ => _.CalculateCommissionAsync(It.IsAny<MoneyTransfer>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}