using System.Threading;
using FluentValidation;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Services;
using Minibank.Core.Domains.Transfers.Validators;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Domains.Transfers.Services
{
    public class CommissionCalculatorTests
    {
        private readonly ICommissionCalculator _commissionCalculator;
        private readonly Mock<IBankAccountRepository> _bankAccountRepositoryMock;
        private readonly Mock<IFractionalNumberEditor> _fractionalNumberEditorMock;

        public CommissionCalculatorTests()
        {
            _bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            _fractionalNumberEditorMock = new Mock<IFractionalNumberEditor>();
            var validator = new MoneyTransferValidator(_bankAccountRepositoryMock.Object);

            _commissionCalculator = new CommissionCalculator(_bankAccountRepositoryMock.Object, validator,
                _fractionalNumberEditorMock.Object);
        }

        private void SetupAssertions(out MoneyTransfer moneyTransfer)
        {
            var bankAccounts = new[] { "SomeBankAccountId_1", "SomeBankAccountId_2" };
            moneyTransfer = new MoneyTransfer()
            {
                Amount = decimal.One,
                Currency = Currency.RUB,
                FromBankAccountId = bankAccounts[0],
                ToBankAccountId = bankAccounts[1]
            };
            _bankAccountRepositoryMock.SetupExist(bankAccounts).Returns(true);
            _bankAccountRepositoryMock.SetupOpenness(bankAccounts).Returns(true);
        }

        private void SetupGetByIdBankAccounts(string fromBankAccountId, string tooBankAccountId)
        {
            var usersId = new[] { "SomeUserId_1", "SomeUserId_2" };
            var bankAccountSender = new BankAccount() { UserId = usersId[0] };
            var bankAccountRecipient = new BankAccount() { UserId = usersId[1] };
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(fromBankAccountId, It.IsAny<CancellationToken>()).Result).Returns(
                    bankAccountSender);
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(tooBankAccountId, It.IsAny<CancellationToken>()).Result).Returns(
                    bankAccountRecipient);
        }
        
        [Fact]
        public async void CalculateCommissionAsync_CalculateCommissionForTransferBetweenAccountsWithDifferentUserid_ShouldReturnRoundValue()
        {
            var expectedResult = decimal.Zero;
            SetupAssertions(out var moneyTransfer);
            SetupGetByIdBankAccounts(moneyTransfer.FromBankAccountId, moneyTransfer.ToBankAccountId);
            _fractionalNumberEditorMock.Setup(_ => _.Round(It.IsAny<decimal>(), It.IsAny<int>())).Returns(expectedResult);

            var actualResult = await _commissionCalculator.CalculateCommissionAsync(moneyTransfer, CancellationToken.None);
            
            Assert.Equal(expectedResult, actualResult);
        }

        [Fact]
        public async void CalculateCommissionAsync_CalculateCommissionForTransferBetweenAccountsWithSameUserid_ShouldReturnZero()
        {
            var usersId = new[] { "SomeUserId_1" };
            var bankAccount = new BankAccount() { UserId = usersId[0] };
            SetupAssertions(out var moneyTransfer);
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()).Result).Returns(
                    bankAccount);

            var actualResult = await _commissionCalculator.CalculateCommissionAsync(moneyTransfer, CancellationToken.None);
            
            Assert.Equal(decimal.Zero, actualResult);
        }

        [Fact]
        public async void CalculateCommissionAsync_CalculateCommission_MethodGetByIdOfBankAccountRepositoryShouldInvokeTwice()
        {
            var usersId = new[] { "SomeUserId_1" };
            var bankAccount = new BankAccount() { UserId = usersId[0] };
            SetupAssertions(out var moneyTransfer);
            _bankAccountRepositoryMock.Setup(
                _ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()).Result).Returns(
                    bankAccount);
            
            await _commissionCalculator.CalculateCommissionAsync(moneyTransfer, CancellationToken.None);
            
            _bankAccountRepositoryMock.Verify(
                _ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        }

        [Fact]
        public async void CalculateCommissionAsync_CalculateCommission_MethodRoundOfFractionalNumberEditorShouldInvokeOnce()
        {
            SetupAssertions(out var moneyTransfer);
            SetupGetByIdBankAccounts(moneyTransfer.FromBankAccountId, moneyTransfer.ToBankAccountId);
            
            await _commissionCalculator.CalculateCommissionAsync(moneyTransfer, CancellationToken.None);
            
            _fractionalNumberEditorMock.Verify(_ => _.Round(It.IsAny<decimal>(), It.IsAny<int>()), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100000)]
        public async void CalculateCommissionAsync_CalculateCommissionForZeroOrNegativeAmount_ShouldThrowValidationException(decimal amount)
        {
            var moneyTransfer = new MoneyTransfer() { Amount = amount };

            await Assert.ThrowsAsync<ValidationException>(
                () => _commissionCalculator.CalculateCommissionAsync(moneyTransfer, CancellationToken.None));
        }

        [Fact]
        public async void CalculateCommissionAsync_CalculateCommissionForSameAccounts_ShouldThrowValidationException()
        {
            var bankAccountId = "SomeBankAccountId";
            var moneyTransfer = new MoneyTransfer()
            {
                Amount = decimal.One,
                FromBankAccountId = bankAccountId,
                ToBankAccountId = bankAccountId
            };

            await Assert.ThrowsAsync<ValidationException>(
                () => _commissionCalculator.CalculateCommissionAsync(moneyTransfer, CancellationToken.None));
        }
    }
}