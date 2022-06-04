using System;
using System.Linq;
using FluentValidation;
using FluentValidation.TestHelper;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Validators;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Domains.Accounts.Validators
{
    public class CreateBankAccountValidationTests
    {
        private readonly IValidator<BankAccount> _validator;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        public CreateBankAccountValidationTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validator = new CreateBankAccountValidator(_userRepositoryMock.Object);
        }

        [Theory]
        [InlineData(0, Currency.RUB)]
        [InlineData(0, Currency.EUR)]
        [InlineData(0, Currency.USD)]
        [InlineData(100, Currency.RUB)]
        [InlineData(100, Currency.EUR)]
        [InlineData(100, Currency.USD)]
        public async void ValidationAsync_SuccessPath_ValidatorShouldNotHaveAnyExceptions(decimal balance, Currency currency)
        {
            var userId = "SomeExistUserId";
            var bankAccount = new BankAccount() { Balance = balance, Currency = currency, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(true);

            var validationResult = await _validator.TestValidateAsync(bankAccount);
            
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async void ValidationAsync_ValidateBankAccount_MethodIsExistOfUserRepositoryShouldInvokeOnceWithSameUserId()
        {
            var userId = "SomeExistUserId";
            var bankAccount = new BankAccount() { Balance = 0, Currency = Currency.RUB, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(true);

            await _validator.TestValidateAsync(bankAccount);
            
            _userRepositoryMock.VerifyExist(userId, Times.Once);
        }

        [Fact]
        public async void ValidationAsync_ValidateBankAccountWithNegativeBalance_MemberBalanceShouldInvalid()
        {
            var userId = "SomeExistUserId";
            var bankAccount = new BankAccount() { Balance = -1, Currency = Currency.RUB, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(true);
            
            var validationResult = await _validator.TestValidateAsync(bankAccount);
            
            validationResult.ShouldHaveValidationErrorFor(account => account.Balance);    
        }

        [Fact]
        public async void ValidationAsync_ValidateBankAccountWithIncorrectCurrency_MemberCurrencyShouldInvalid()
        {
            var userId = "SomeExistUserId";
            var incorrectCurrency = Enum.GetValues<Currency>().Max() + 1;
            var bankAccount = new BankAccount() { Balance = 0, Currency = incorrectCurrency, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(true);
            
            var validationResult = await _validator.TestValidateAsync(bankAccount);
            
            validationResult.ShouldHaveValidationErrorFor(account => account.Currency); 
        }

        [Fact]
        public async void ValidationAsync_ValidateBankAccountOfNotExistUser_MemberUserIdShouldInvalid()
        {
            var userId = "SomeNotExistUserId";
            var bankAccount = new BankAccount() { Balance = 0, Currency = Currency.RUB, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(false);
            
            var validationResult = await _validator.TestValidateAsync(bankAccount);
            
            validationResult.ShouldHaveValidationErrorFor(account => account.UserId);
        }
    }
}