using FluentValidation;
using FluentValidation.TestHelper;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Validators;
using Xunit;

namespace Minibank.Core.Tests.Domains.Accounts.Validators
{
    public class CloseBankAccountValidatorTests
    {
        private readonly IValidator<BankAccount> _validator;

        public CloseBankAccountValidatorTests()
        {
            _validator = new CloseBankAccountValidator();
        }
        
        [Fact]
        public async void ValidationAsync_SuccessPath_ValidatorShouldNotHaveAnyExceptions()
        {
            var bankAccount = new BankAccount() { Balance = 0m, IsOpen = true };

            var validationResult = await _validator.TestValidateAsync(bankAccount);
            
            validationResult.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async void ValidationAsync_ValidateClosedBankAccount_MemberIsOpenShouldInvalid()
        {
            var bankAccount = new BankAccount() { Balance = 0m, IsOpen = false };

            var validationResult = await _validator.TestValidateAsync(bankAccount);

            validationResult.ShouldHaveValidationErrorFor(account => account.IsOpen);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(1)]
        public async void ValidationAsync_ValidateBankAccountWithNotZeroBalance_MemberBalanceShouldInvalid(decimal balance)
        {
            var bankAccount = new BankAccount() { Balance = balance, IsOpen = true };

            var validationResult = await _validator.TestValidateAsync(bankAccount);

            validationResult.ShouldHaveValidationErrorFor(account => account.Balance);
        }
    }
}