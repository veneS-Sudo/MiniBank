using System;
using System.Collections.Generic;
using System.Threading;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Accounts.Validators;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Core.Tests.Domains.Accounts.EqualityComparers;
using Minibank.Core.Tests.Mocks.MockExtensions;
using Minibank.Core.UniversalValidators;
using Moq;
using Xunit;
using ValidationException = FluentValidation.ValidationException;

namespace Minibank.Core.Tests.Domains.Accounts.Services
{
    public class BankAccountServiceTests
    {
        private readonly IBankAccountService _accountService;
        private readonly Mock<IBankAccountRepository> _accountRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public BankAccountServiceTests()
        {
            _accountRepositoryMock = new Mock<IBankAccountRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            
            var createBankAccountValidator = new CreateBankAccountValidator(_userRepositoryMock.Object);
            var closeBankAccountValidator = new CloseBankAccountValidator();
            var updateBankAccountValidator = new UpdateBankAccountValidator(_accountRepositoryMock.Object);
            var idValidator = new IdEntityValidator();

            _accountService = new BankAccountService(_accountRepositoryMock.Object, _unitOfWorkMock.Object, createBankAccountValidator,
                closeBankAccountValidator, idValidator, updateBankAccountValidator);
        }
        
        [Fact]
        public async void GetByIdAsync_GetBankAccountByExistId_ShouldReturnUserWithSameId()
        {
            var id = "SomeExistId";
            var bankAccount = new BankAccount() { Id = id };
            _accountRepositoryMock.Setup(repository => repository.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(bankAccount);

            var gotAccount = await _accountService.GetByIdAsync(id, CancellationToken.None);
            
            Assert.Equal(id, gotAccount.Id);
        }

        [Fact]
        public async void GetByIdAsync_GetBankAccountByExistId_MethodGetByIdOfAccountRepositoryShouldInvocationOnce()
        {
            var id = "SomeExistId";
            
            await _accountService.GetByIdAsync(id, CancellationToken.None);
            
            _accountRepositoryMock.Verify(repository => repository.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async void GetById_GetBankAccount_GetByIdOfAccountRepositoryShouldInvokeWithSameId()
        {
            var id = "SomeExistId";
            
            await _accountService.GetByIdAsync(id, CancellationToken.None);

            _accountRepositoryMock.Verify(
                _ => _.GetByIdAsync(It.Is<string>(inId => inId == id), It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void GetByIdAsync_GetByBankAccountByNotExistId_ShouldThrowObjectNotFoundException()
        {
            var id = "SomeNotExistId";
            
            _accountRepositoryMock.Setup(
                repository => repository.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()).Result)
                .Throws<ObjectNotFoundException>();

            await Assert.ThrowsAsync<ObjectNotFoundException>(() => _accountService.GetByIdAsync(id, CancellationToken.None));
        }
        
        [Fact]
        public async void GetAllAccountAsync_GetAllBankAccount_ShouldReturnNotEmpty()
        {
            var accounts = new List<BankAccount> { new() };
            _accountRepositoryMock.Setup(repository => repository.GetAllAccountsAsync(It.IsAny<CancellationToken>()).Result)
                .Returns(accounts);
        
            var actual = await _accountService.GetAllAccountsAsync(CancellationToken.None);
        
            Assert.NotEmpty(actual);
        }

        [Fact]
        public async void GetAllAccountAsync_GetAllBankAccount_ShouldReturnSequenceSameLength()
        {
            var accounts = new List<BankAccount> { new (), new () };
            _accountRepositoryMock.Setup(repository => repository.GetAllAccountsAsync(It.IsAny<CancellationToken>()).Result)
                .Returns(accounts);
        
            var actual = await _accountService.GetAllAccountsAsync(CancellationToken.None);
        
            Assert.Equal(accounts.Count, actual.Count);
        }

        [Fact]
        public async void GetAllAccountAsync_GetAllBankAccount_ShouldReturnSameSequence()
        {
            var expectedAccounts = new List<BankAccount>()
            {
                new () { UserId = "User_1", Balance = Decimal.Zero, Currency = Currency.RUB, DateOpen = DateTime.MinValue},
                new () { UserId = "User_2", Balance = Decimal.One, Currency = Currency.USD, DateOpen = DateTime.MinValue}
            };
            _accountRepositoryMock.Setup(_ => _.GetAllAccountsAsync(It.IsAny<CancellationToken>()).Result)
                .Returns(expectedAccounts);

            var actualAccounts = await _accountService.GetAllAccountsAsync(CancellationToken.None);
            
            Assert.Equal(expectedAccounts, actualAccounts, new BankAccountWithoutIdEqualityComparer());
        }

        [Fact]
        public async void GetAllAccountAsync_GetAllBankAccount_MethodGetAllAccountOfAccountRepositoryShouldInvokeOnce()
        {
            // Act
            await _accountService.GetAllAccountsAsync(CancellationToken.None);
            
            // Assert
            _accountRepositoryMock.Verify(_ => _.GetAllAccountsAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CreateAccountAsync_CreateValidBankAccount_ShouldReturnCreatedBankAccount()
        {
            var userId = "SomeExistUserId";
            var account = new BankAccount() {Balance = 0m, Currency = Currency.RUB, UserId = userId };
            var expectedAccount = new BankAccount() { Id = "SomeId", Balance = 0m, Currency = Currency.RUB, UserId = userId, DateOpen = DateTime.Today };
            _accountRepositoryMock.Setup(
                repository => repository.CreateAccountAsync(account, It.IsAny<CancellationToken>()).Result)
                .Returns(expectedAccount);
            _userRepositoryMock.SetupExist(userId).Returns(true);
            
            var actual = await _accountService.CreateAccountAsync(account, CancellationToken.None);
            
            Assert.Equal(expectedAccount, actual, new BankAccountWithoutIdEqualityComparer());
        }

        [Fact]
        public async void CreateAccountAsync_CreateValidBankAccount_MethodCreateOfAccountRepositoryShouldInvokeOnce()
        {
            var userId = "SomeExistUserId";
            var account = new BankAccount() {Balance = 0m, Currency = Currency.RUB, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(true);
            
            await _accountService.CreateAccountAsync(account, CancellationToken.None);

            _accountRepositoryMock.Verify(
                _ => _.CreateAccountAsync(account, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CreateAccountAsync_CreateValidBankAccount_MethodCreateOfAccountRepositoryShouldInvokeWithSameAccount()
        {
            var userId = "SomeExistUserId";
            var bankAccount = new BankAccount() {Balance = 0m, Currency = Currency.RUB, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(true);

            await _accountService.CreateAccountAsync(bankAccount, CancellationToken.None);
            
            _accountRepositoryMock.Verify(_ => _.CreateAccountAsync(It.Is(bankAccount, new BankAccountWithoutIdEqualityComparer()),
                        It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void CreateAccountAsync_CreateValidBankAccount_MethodIsExistOfUserRepositoryShouldInvokeOnce()
        {
            var userId = "SomeExistUserId";
            var account = new BankAccount() {Balance = 0m, Currency = Currency.RUB, UserId = userId};
            _userRepositoryMock.SetupExist(userId).Returns(true);
            
            await _accountService.CreateAccountAsync(account, CancellationToken.None);
            
            _userRepositoryMock.Verify(_ => _.IsExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CreateAccountAsync_CreateValidBankAccount_MethodIsExistOfUserRepositoryShouldInvokeWithSameUserId()
        {
            var userId = "SomeExistUserId";
            var account = new BankAccount() {Balance = 0m, Currency = Currency.RUB, UserId = userId};
            _userRepositoryMock.SetupExist(userId).Returns(true);
            
            await _accountService.CreateAccountAsync(account, CancellationToken.None);
            
            _userRepositoryMock.Verify(_ => _.IsExistAsync(userId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void CreateAccountAsync_CreateValidBankAccount_MethodSaveOfUnitOfWorkShouldInvokeOnce()
        {
            var userId = "SomeExistUserId";
            var account = new BankAccount() {Balance = 0m, Currency = Currency.RUB, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(true);
            
            await _accountService.CreateAccountAsync(account, CancellationToken.None);
            
            _unitOfWorkMock.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void CreateAccountAsync_CreateBankAccountByNotExistUser_ThrowValidationException()
        {
            var userId = "SomeNotExistUserId";
            var account = new BankAccount() { Balance = 0m, Currency = Currency.RUB, UserId = userId };
            _userRepositoryMock.SetupExist(userId).Returns(false);

            await Assert.ThrowsAsync<ValidationException>(
                () => _accountService.CreateAccountAsync(account, CancellationToken.None));
        }
        
        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_ShouldReturnTrue()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            _accountRepositoryMock.Setup(_ => _.UpdateAccountAsync(account, It.IsAny<CancellationToken>()).Result)
                .Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(1);
            
            var actual = await _accountService.UpdateAccountAsync(account, CancellationToken.None);
            
            Assert.True(actual);
        }

        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_MethodUpdateOfAccountRepositoryShouldInvokeOnce()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);

            await _accountService.UpdateAccountAsync(account, CancellationToken.None);

            _accountRepositoryMock.Verify(
                _ => _.UpdateAccountAsync(It.IsAny<BankAccount>(), It.IsAny<CancellationToken>()), Times.Once);
        }
        
        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_MethodUpdateOfAccountRepositoryShouldInvokeWithSomeBankAccount()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);

            await _accountService.UpdateAccountAsync(account, CancellationToken.None);

            _accountRepositoryMock.Verify( _ => _.UpdateAccountAsync(It.Is(account, new BankAccountWithoutIdEqualityComparer()),
                        It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_MethodSaveOfUnitOfWorkShouldInvokeOnce()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            
            await _accountService.UpdateAccountAsync(account, CancellationToken.None);

            _unitOfWorkMock.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_MethodIsExistOfAccountRepositoryShouldInvokeOnce()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            
            await _accountService.UpdateAccountAsync(account, CancellationToken.None);

            _accountRepositoryMock.Verify(_ => _.IsExistAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_MethodIsExistOfAccountRepositoryShouldInvokeWithSame()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            
            await _accountService.UpdateAccountAsync(account, CancellationToken.None);
            
            _accountRepositoryMock.Verify(_ => _.IsExistAsync(accountId, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_MethodIsOpenOfAccountRepositoryShouldInvokeOnce()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            
            await _accountService.UpdateAccountAsync(account, CancellationToken.None);

            _accountRepositoryMock.Verify(_ => _.IsOpenAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void UpdateAccountAsync_UpdateValidBankAccount_MethodIsOpenOfAccountRepositoryShouldInvokeWithSameId()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            
            await _accountService.UpdateAccountAsync(account, CancellationToken.None);
            
            _accountRepositoryMock.Verify(_ => _.IsOpenAsync(accountId, It.IsAny<CancellationToken>()));
        }
        
        [Fact]
        public async void UpdateAccountAsync_UpdateNotExistBankAccount_ThrowValidationException()
        {
            var accountId = "SomeNotExistId";
            var account = new BankAccount() { Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(false);
            
            await Assert.ThrowsAsync<ValidationException>(
                () => _accountService.UpdateAccountAsync(account, CancellationToken.None));
        }

        [Fact]
        public async void UpdateAccountAsync_UpdateNotOpennessBankAccount_ThrowValidationException()
        {
            var accountId = "SomeExistNotOpenId";
            var account = new BankAccount() { Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(false);
            
            await Assert.ThrowsAsync<ValidationException>(
                () => _accountService.UpdateAccountAsync(account, CancellationToken.None));
        }

        [Fact]
        public async void UpdateAccountAsync_AccountRepositoryUnsuccessfulUpdateValidBankAccount_ShouldReturnFalse()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            _accountRepositoryMock.Setup(_ => _.UpdateAccountAsync(account, It.IsAny<CancellationToken>()).Result)
                .Returns(false);
            
            var actual = await _accountService.UpdateAccountAsync(account, CancellationToken.None);
            
            Assert.False(actual);
        }
        
        [Fact]
        public async void UpdateAccountAsync_UnitOfWorkUnsuccessfulUpdateValidBankAccount_ShouldReturnFalse()
        {
            var accountId = "SomeExistId";
            var account = new BankAccount() { Balance = 100m, Id = accountId };
            _accountRepositoryMock.SetupExist(accountId).Returns(true);
            _accountRepositoryMock.SetupOpenness(accountId).Returns(true);
            _accountRepositoryMock.Setup(_ => _.UpdateAccountAsync(account, It.IsAny<CancellationToken>()).Result)
                .Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(0);
            
            var actual = await _accountService.UpdateAccountAsync(account, CancellationToken.None);
            
            Assert.False(actual);    
        }

        [Fact]
        public async void CloseAccountAsync_CloseValidBankAccount_ShouldReturnTrue()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            _accountRepositoryMock.Setup(_ => _.CloseAccountAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(1);
            
            var closingResult = await _accountService.CloseAccountAsync(id, CancellationToken.None);

            Assert.True(closingResult);
        }

        [Fact]
        public async void CloseAccountAsync_CloseValidBankAccount_MethodGetByIdOfAccountRepositoryShouldInvokeOnce()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            
            await _accountService.CloseAccountAsync(id, CancellationToken.None);

            _accountRepositoryMock.Verify(_ => _.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void CloseAccountAsync_CloseValidBankAccount_MethodGetByIdOfAccountRepositoryShouldInvokeWithSameId()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            
            await _accountService.CloseAccountAsync(id, CancellationToken.None);

            _accountRepositoryMock.Verify(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void CloseAccountAsync_CloseValidBankAccount_MethodCloseAccountOfAccountRepositoryShouldInvokeOnce()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            
            await _accountService.CloseAccountAsync(id, CancellationToken.None);

            _accountRepositoryMock.Verify(_ => _.CloseAccountAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()),
                Times.Once);
        }
        
        [Fact]
        public async void CloseAccountAsync_CloseValidBankAccount_MethodCloseAccountOfAccountRepositoryShouldInvokeWithSameId()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            
            await _accountService.CloseAccountAsync(id, CancellationToken.None);

            _accountRepositoryMock.Verify(_ => _.CloseAccountAsync(id, It.IsAny<CancellationToken>()));
        }

        [Fact]
        public async void CloseAccountAsync_CloseValidBankAccount_MethodSaveOFUnitOfWorkShouldInvokeOnce()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            
            await _accountService.CloseAccountAsync(id, CancellationToken.None);
            
            _unitOfWorkMock.Verify(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async void CloseAccountAsync_CloseNotExistBankAccount_ThrowObjectNotFoundException()
        {
            var id = "SomeExistId";
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Throws<ObjectNotFoundException>();

            await Assert.ThrowsAsync<ObjectNotFoundException>(
                () => _accountService.CloseAccountAsync(id, CancellationToken.None));
        }

        [Fact]
        public async void CloseAccountAsync_CloseInvalidBankAccount_ThrowValidationException()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 100, IsOpen = false };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);

            await Assert.ThrowsAsync<ValidationException>(
                () => _accountService.CloseAccountAsync(id, CancellationToken.None));
        }

        [Fact]
        public async void CloseAccountAsync_RepositoryUnsuccessfulCloseValidBankAccount_ShouldReturnFalse()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            _accountRepositoryMock.Setup(_ => _.CloseAccountAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(false);

            var closingResult =  await _accountService.CloseAccountAsync(id, CancellationToken.None);
            
            Assert.False(closingResult);
        }

        [Fact]
        public async void CloseAccountAsync_UnitOfWorkUnsuccessfulCloseValidBankAccount_ShouldReturnFalse()
        {
            var id = "SomeExistId";
            var account = new BankAccount() { Id = id, Balance = 0, IsOpen = true };
            _accountRepositoryMock.Setup(_ => _.GetByIdAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(account);
            _accountRepositoryMock.Setup(_ => _.CloseAccountAsync(id, It.IsAny<CancellationToken>()).Result)
                .Returns(true);
            _unitOfWorkMock.Setup(_ => _.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(0);
            
            var closingResult =  await _accountService.CloseAccountAsync(id, CancellationToken.None);
            
            Assert.False(closingResult);
        }
    }
}