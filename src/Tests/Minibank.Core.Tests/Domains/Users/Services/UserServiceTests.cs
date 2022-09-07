using System.Collections.Generic;
using System.Threading;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Services;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Core.Tests.Domains.Users.EqualityComparers;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Minibank.Core.UniversalValidators;
using Moq;
using Xunit;

namespace Minibank.Core.Tests.Domains.Users.Services
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IBankAccountRepository> _bankAccountRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _bankAccountRepositoryMock = new Mock<IBankAccountRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            var loggerMock = new Mock<ILogger<UserService>>();
            var idValidator = new IdEntityValidator();
            var createUserValidator = new CreateUserValidator();
            var updateUserValidator = new UpdateUserValidator(_userRepositoryMock.Object);
            
            _userService = new UserService(_userRepositoryMock.Object, _bankAccountRepositoryMock.Object, _unitOfWorkMock.Object, idValidator,
                createUserValidator, updateUserValidator, loggerMock.Object);
        }

        [Fact]
        public async void GetByIdAsync_GetUserByExistId_ShouldReturnUserWithSameId()
        {
            var id = "SomeExistId";
            var expected = new User() { Id = id };
            _userRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(expected);

            var actual = await _userService.GetByIdAsync(id, CancellationToken.None);

            Assert.Same(expected, actual);
        }

        [Fact]
        public async void GetByIdAsync_GetUserByExistId_MethodGetByIdOfUserRepositoryShouldInvokeOnce()
        {
            var id = "SomeExistId";
            
            await _userService.GetByIdAsync(id, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);    
        }
        
        [Fact]
        public async void GetByIdAsync_GetUserByExistId_MethodGetByIdOfUserRepositoryShouldInvokeWithSameId()
        {
            var id = "SomeExistId";
            
            await _userService.GetByIdAsync(id, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()));    
        }

        [Fact]
        public async void GetByIdAsync_GetByNotExistId_ThrowObjectNotFoundException()
        {
            var id = "SomeExistId";
            _userRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Throws<ObjectNotFoundException>();

            await Assert.ThrowsAsync<ObjectNotFoundException>(
                () => _userService.GetByIdAsync(id, CancellationToken.None));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void GetByIdAsync_GetByInvalidId_ThrowValidationException(string id)
        {
            var userId = id;

            await Assert.ThrowsAsync<ValidationException>(() => _userService.GetByIdAsync(userId, CancellationToken.None));
        }
        
        [Fact]
        public async void GetAllUsersAsync_GetAllUsersFromNotEmptyStorage_ShouldReturnNotEmptyCollection()
        {
            var users = new List<User>() { new() };
            _userRepositoryMock.Setup(_ => _.GetAllUsersAsync(It.IsAny<CancellationToken>()).Result).Returns(
                users);

            var actualUsers = await _userService.GetAllUsersAsync(CancellationToken.None);
            
            Assert.NotNull(actualUsers);
            Assert.NotEmpty(actualUsers);
        }

        [Fact]
        public async void GetAllUsersAsync_GetAllUsersFromNotEmptyStorage_ShouldReturnNotEmptyCollectionWithSameCount()
        {
            var expectedUsers = new List<User>() { new(), new() };
            _userRepositoryMock.Setup(_ => _.GetAllUsersAsync(It.IsAny<CancellationToken>()).Result).Returns(
                expectedUsers);

            var actualUsers = await _userService.GetAllUsersAsync(CancellationToken.None);
            
            Assert.NotNull(actualUsers);
            Assert.Equal(expectedUsers.Count, actualUsers.Count);
        }

        [Fact]
        public async void GetAllUsersAsync_GetAllUsersFromNotEmptyStorage_ShouldReturnSameSequence()
        {
            var expectedUsers = new List<User>()
            {
                new() { Login = "Login_1", Email = "email_1@test.ru" },
                new() { Login = "Login_2", Email = "email_2@test.com" },
                new() { Login = "Login_0" }
            };
            _userRepositoryMock.Setup( _ => _.GetAllUsersAsync(It.IsAny<CancellationToken>()).Result).Returns(
                expectedUsers);

            var actualUsers = await _userService.GetAllUsersAsync(CancellationToken.None);
            
            Assert.NotNull(actualUsers);
            Assert.Equal(expectedUsers, actualUsers, new UserWithoutIdEqualityComparer());
        }
            
        [Fact]
        public async void GetAllUsersAsync_GetAllUsers_MethodGetAllUsersOfUserRepositoryShouldInvokeOnce()
        {
            // Act
            await _userService.GetAllUsersAsync(CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(_ => _.GetAllUsersAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CreateUserAsync_CreateUserWithValidModel_ShouldReturnCreatedUserWithNotEmptyId()
        {
            var user = new User() { Login = "SomeLogin", Email = "some@email.com" };
            _userRepositoryMock.Setup(
                repository => repository.CreateUserAsync(user, It.IsAny<CancellationToken>()).Result).Returns(
                    () => new User() { Id = "SomeId" });

            var createUser = await _userService.CreateUserAsync(user, CancellationToken.None);
            
            Assert.NotNull(createUser);
            Assert.NotEmpty(createUser.Id);
        }

        [Fact]
        public async void CreateUserAsync_CreateUserWithValidModel_ShouldReturnCreateUserWithSameValues()
        {
            var expectedUser = new User() { Login = "SomeLogin", Email = "some@email.com" };
            _userRepositoryMock.Setup(
                repository => repository.CreateUserAsync(expectedUser, It.IsAny<CancellationToken>()).Result).Returns(
                    () => new User()
                    {
                        Login = expectedUser.Login,
                        Email = expectedUser.Email
                    });

            var actualUser = await _userService.CreateUserAsync(expectedUser, CancellationToken.None);    
            
            Assert.Equal(expectedUser, actualUser, new UserWithoutIdEqualityComparer());
        }

        [Fact]
        public async void CreateUserAsync_CreateUserWithValidModel_MethodSaveOfUnitIfWorkShouldInvokeOnce()
        {
            var user = new User() { Login = "SomeLogin", Email = "some@email.com" };

            await _userService.CreateUserAsync(user, CancellationToken.None);

            _unitOfWorkMock.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CreateUserAsync_CreateUserWithInvalidModel_ThrowValidationException()
        {
            var user = new User() { Login = "", Email = null };

            await Assert.ThrowsAsync<ValidationException>(
                () => _userService.CreateUserAsync(user, CancellationToken.None));
        }

        [Fact]
        public async void UpdateUserAsync_UpdateValidAndExistUser_ShouldReturnTrue()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            _userRepositoryMock.Setup(_ => _.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()).Result)
                .Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(1);
        
            var actual = await _userService.UpdateUserAsync(user, CancellationToken.None);
        
            Assert.True(actual);
        }

        [Fact]
        public async void UpdateUserAsync_UpdateValidAndExistUser_MethodUpdateUserOfUserRepositoryShouldInvokeOnce()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            
            await _userService.UpdateUserAsync(user, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void UpdateUserAsync_UpdateValidAndExistUser_MethodUpdateUserOfUserRepositoryShouldInvokeWithSameUser()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            
            await _userService.UpdateUserAsync(user, CancellationToken.None);

            _userRepositoryMock.Verify(
                _ => _.UpdateUserAsync(It.Is(user, new UserWithoutIdEqualityComparer()), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateUserAsync_UpdateValidAndExistUser_MethodIsExistOfUserRepositoryShouldInvokeOnce()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            
            await _userService.UpdateUserAsync(user, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.IsExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void UpdateUserAsync_UpdateValidAndExistUser_MethodIsExistOfUserRepositoryShouldInvokeWithSameId()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            
            await _userService.UpdateUserAsync(user, CancellationToken.None);
            
            _userRepositoryMock.Verify(_ => _.IsExistAsync(user.Id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateUserAsync_UpdateValidAndExistUser_MethodSaveOfUnitOfWorkShouldInvokeOnce()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            
            await _userService.UpdateUserAsync(user, CancellationToken.None);
            
            _unitOfWorkMock.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void UpdateUserAsync_UpdateInvalidExistUser_ThrowValidationException()
        {
            var user = new User() { Id = "", Login = null };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);

            await Assert.ThrowsAsync<ValidationException>(
                () => _userService.UpdateUserAsync(user, CancellationToken.None));
        }

        [Fact]
        public async void UpdateUserAsync_UpdateValidNotExistUser_ThrowValidationException()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(false);

            await Assert.ThrowsAsync<ValidationException>(
                () => _userService.UpdateUserAsync(user, CancellationToken.None));
        }

        [Fact]
        public async void UpdateUserAsync_UserRepositoryUnsuccessfulUpdateValidExistUser_ShouldReturnFalse()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            _userRepositoryMock.Setup(_ => _.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()).Result)
                .Returns(false);

            var actual = await _userService.UpdateUserAsync(user, CancellationToken.None);
        
            Assert.False(actual);
        }

        [Fact]
        public async void UpdateUserAsync_UnitOfWorkUnsuccessfulUpdateValidExistUser_ShouldReturnFalse()
        {
            var user = new User() { Id = "SomeId", Login = "SomeLogin" };
            _userRepositoryMock.SetupExist(user.Id).Returns(true);
            _userRepositoryMock.Setup(_ => _.UpdateUserAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()).Result)
                .Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(0);

            var actual = await _userService.UpdateUserAsync(user, CancellationToken.None);
        
            Assert.False(actual);
        }

        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_ShouldReturnTrue()
        {
            string userId = "SomeId";
            _userRepositoryMock.Setup(_ => _.DeleteUserAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(1);
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            var actual = await _userService.DeleteUserAsync(userId, CancellationToken.None);
            
            Assert.True(actual);
        }

        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_MethodDeleteUserOfUserRepositoryShouldInvokeOnce()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.DeleteUserAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_MethodDeleteUserOfUserRepositoryShouldInvokeWithSameUserId()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.DeleteUserAsync(userId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_MethodIsExistOfUserRepositoryShouldInvokeOnce()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.IsExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_MethodIsExistOfUserRepositoryShouldInvokeWithSameUserId()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

            _userRepositoryMock.Verify(_ => _.IsExistAsync(userId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_MethodExistByUserIdOfBankAccountRepositoryShouldInvokeOnce()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(_ => _.ExistByUserIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_MethodExistByUserIdOfBankAccountRepositoryShouldInvokeWithSameUserId()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await _userService.DeleteUserAsync(userId, CancellationToken.None);

            _bankAccountRepositoryMock.Verify(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithoutBankAccount_MethodSaveOfUnitOfWorkShouldInvokeOnce()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            await _userService.DeleteUserAsync(userId, CancellationToken.None);
            
            _unitOfWorkMock.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void DeleteUserAsync_DeleteNotExistUser_ThrowValidationException()
        {
            string userId = "SomeNotExistId";
            _userRepositoryMock.SetupExist(userId).Returns(false);

            await Assert.ThrowsAsync<ObjectNotFoundException>(
                () => _userService.DeleteUserAsync(userId, CancellationToken.None));
        }

        [Fact]
        public async void DeleteUserAsync_DeleteExistUserWithBankAccount_ThrowValidationException()
        {
            string userId = "SomeId";
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(true);   
            
            await Assert.ThrowsAsync<ValidationException>(
                () => _userService.DeleteUserAsync(userId, CancellationToken.None));
        }

        [Fact]
        public async void DeleteUserAsync_UserRepositoryUnsuccessfulDeleteExistUserWithoutBankAccount_ShouldReturnFalse()
        {
            string userId = "SomeId";
            _userRepositoryMock.Setup(_ => _.DeleteUserAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(0);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            var actual = await _userService.DeleteUserAsync(userId, CancellationToken.None);
            
            Assert.False(actual);        
        }

        [Fact]
        public async void DeleteUserAsync_UnitOfWorkUnsuccessfulDeleteExistUserWithoutBankAccount_ShouldReturnFalse()
        {
            string userId = "SomeId";
            _userRepositoryMock.Setup(_ => _.DeleteUserAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            _userRepositoryMock.SetupExist(userId).Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(0);
            _bankAccountRepositoryMock.Setup(_ => _.ExistByUserIdAsync(userId, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            var actual = await _userService.DeleteUserAsync(userId, CancellationToken.None);
            
            Assert.False(actual);
        }
       }
}