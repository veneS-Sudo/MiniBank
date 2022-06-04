using FluentValidation;
using FluentValidation.TestHelper;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Validators;
using Xunit;

namespace Minibank.Core.Tests.Domains.Users.Validators
{
    public class CreateUserValidatorTests
    {
        private readonly IValidator<User> _validator;
        public CreateUserValidatorTests()
        {
            _validator = new CreateUserValidator();
        }

        [Fact]
        public async void ValidateAsync_SuccessPath_UserShouldValid()
        {
            var user = new User() { Login = "SomeLogin", Email = "email@mail.ru" };

            var validationResult = await _validator.TestValidateAsync(user);
            
            validationResult.ShouldNotHaveAnyValidationErrors();
        }
        
        [Theory]
        [ClassData(typeof(LoginDataGenerator))]
        public async void ValidateAsync_ValidateUser_MemberLoginShouldInvalid(string login)
        {
            var user = new User() { Login = login, Email = "some@mail.com" };
            
            var validationResult = await _validator.TestValidateAsync(user);

            validationResult.ShouldHaveValidationErrorFor(userModel => userModel.Login);    
        }
        
        [Theory]
        [ClassData(typeof(EmailDataGenerator))]
        public async void ValidateAsync_ValidateUser_MemberEmailShouldInvalid(string email)
        {
            var user = new User() { Login = "SomeLogin", Email = email };
            
            var validationResult = await _validator.TestValidateAsync(user);

            validationResult.ShouldHaveValidationErrorFor(userModel => userModel.Email);
        }
    }
}