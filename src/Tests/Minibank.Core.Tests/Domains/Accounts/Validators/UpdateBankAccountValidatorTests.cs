using System.Threading;
using FluentValidation;
using FluentValidation.TestHelper;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Accounts.Validators;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Domains.Accounts.Validators
{
    public class UpdateBankAccountValidatorTests
    {
        private readonly IValidator<BankAccount> _validator;
        private readonly Mock<IBankAccountRepository> _bankRepositoryMock;

        public UpdateBankAccountValidatorTests()
        {
            _bankRepositoryMock = new Mock<IBankAccountRepository>();
            _validator = new UpdateBankAccountValidator(_bankRepositoryMock.Object);
        }

        [Fact]
        public async void ValidationAsync_SuccessPath_BankAccountShouldValid()
        {
            var id = "SomeExistId";
            var bankAccount = new BankAccount() {Id = id};
            _bankRepositoryMock.SetupExist(id).Returns(true);
            _bankRepositoryMock.SetupOpenness(id).Returns(true);

            var validationResult = await _validator.TestValidateAsync(bankAccount);

            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async void ValidationAsync_ValidateBankAccount_MethodIsExistOfBankAccountShouldInvokeOnceWithSameId()
        {
            var id = "SomeExistId";
            var bankAccount = new BankAccount() {Id = id};
            _bankRepositoryMock.SetupExist(id).Returns(true);
            _bankRepositoryMock.SetupOpenness(id).Returns(true);

            await _validator.TestValidateAsync(bankAccount);

            _bankRepositoryMock.VerifyExist(id, Times.Once);
        }

        [Fact]
        public async void ValidationAsync_ValidateBankAccount_MethodIsOpenOfBankAccountShouldInvokeOnceWithSameId()
        {
            var id = "SomeExistId";
            var bankAccount = new BankAccount() {Id = id};
            _bankRepositoryMock.SetupExist(id).Returns(true);
            _bankRepositoryMock.SetupOpenness(id).Returns(true);

            await _validator.TestValidateAsync(bankAccount);

            _bankRepositoryMock.VerifyOpenness(id, Times.Once);
        }

        [Fact]
        public async void ValidationAsync_ValidateNotExistBankAccount_MemberIdShouldInvalid()
        {
            var id = "SomeNotExistId";
            var bankAccount = new BankAccount() {Id = id};
            _bankRepositoryMock.SetupExist(id).Returns(false);

            var validationResult = await _validator.TestValidateAsync(bankAccount);

            validationResult.ShouldHaveValidationErrorFor(account => account.Id);
        }
        
        [Fact]
        public async void ValidationAsync_ValidateClosedBankAccount_MemberIdShouldInvalid()
        {
            var id = "SomeExistId";
            var bankAccount = new BankAccount() {Id = id};
            _bankRepositoryMock.SetupExist(id).Returns(true);
            _bankRepositoryMock.SetupOpenness(id).Returns(false);
            
            var validationResult = await _validator.TestValidateAsync(bankAccount);

            validationResult.ShouldHaveValidationErrorFor(account => account.Id);
        }

        [Fact]
        public async void ValidationAsync_ValidateNotExistBankAccount_MethodIsOpenOfBankAccountShouldNotInvoke()
        {
            var id = "SomeNotExistId";
            var bankAccount = new BankAccount() {Id = id};
            _bankRepositoryMock.SetupExist(id).Returns(false);

            await _validator.TestValidateAsync(bankAccount);

            _bankRepositoryMock.Verify(_ => _.IsOpenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Never);
        }
    }
}