using FluentValidation;
using FluentValidation.TestHelper;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Domains.Users.Validators
{
    public class UpdateUserValidatorTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly IValidator<User> _validator;
        
        public UpdateUserValidatorTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _validator = new UpdateUserValidator(_userRepositoryMock.Object);
        }

        [Fact]
        public async void ValidateAsync_SuccessPath_UserShouldValid()
        {
            var id = "SomeExistId";
            var user = new User() { Id = id, Login = "SomeLogin", Email = "some@mail.com" };
            _userRepositoryMock.SetupExist(id).Returns(true);

            var resultValidation = await _validator.TestValidateAsync(user);
            
            resultValidation.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public async void ValidateAsync_ValidateUser_MethodIsExistOfUserRepositoryShouldInvokeOnceWithSameId()
        {
            var id = "SomeExistId";
            var user = new User() { Id = id };

            await _validator.TestValidateAsync(user);
            
            _userRepositoryMock.VerifyExist(id, Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void ValidateAsync_ValidateUser_MemberIdShouldInvalid(string id)
        {
            var user = new User() { Id = id, Login = "SomeLogin", Email = "some@mail.com" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);

            var validationResult = await _validator.TestValidateAsync(user);

            validationResult.ShouldHaveValidationErrorFor(userModel => userModel.Id);
        }

        [Theory]
        [ClassData(typeof(LoginDataGenerator))]
        public async void ValidateAsync_ValidateUser_MemberLoginShouldInvalid(string login)
        {
            var id = "SomeId";
            var user = new User() { Id = id, Login = login, Email = "some@mail.com" };
            _userRepositoryMock.SetupExist(id).Returns(true);

            var validationResult = await _validator.TestValidateAsync(user);

            validationResult.ShouldHaveValidationErrorFor(userModel => userModel.Login);    
        }

        [Theory]
        [ClassData(typeof(EmailDataGenerator))]
        public async void ValidateAsync_ValidateUser_MemberEmailShouldInvalid(string email)
        {
            var id = "SomeId";
            var user = new User() { Id = id, Login = "SomeLogin", Email = email };
            _userRepositoryMock.SetupExist(id).Returns(true);
            
            var validationResult = await _validator.TestValidateAsync(user);

            validationResult.ShouldHaveValidationErrorFor(userModel => userModel.Email);
        }
    }
}