using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Core.UniversalValidators;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _bankAccountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IdEntityValidator _idValidator;
        private readonly CreateUserValidator _createUserValidator;
        private readonly UpdateUserValidator _updateUserValidator;

        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, IBankAccountRepository bankAccountRepository, IUnitOfWork unitOfWork, IdEntityValidator idValidator,
            CreateUserValidator createUserValidator, UpdateUserValidator updateUserValidator, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _bankAccountRepository = bankAccountRepository;
            _unitOfWork = unitOfWork;
            _idValidator = idValidator;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
            _logger = logger;
        }
        
        public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            var user = await _userRepository.GetByIdAsync(id, cancellationToken);
            
            _logger.LogInformation("User received, Id={Id}, Login={Login}, Email={Email}", user?.Id, user?.Login, user?.Email);
            
            return user;
        }

        public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetAllUsersAsync(cancellationToken);
            _logger.LogInformation("Users received, Count={Count}", users?.Count);

            return users;
        }

        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            await _createUserValidator.ValidateAndThrowAsync(user, cancellationToken);
            var createUser = await _userRepository.CreateUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User created, Id={Id}, Login={Login}, Email={Email}", 
                createUser?.Id, createUser?.Login, createUser?.Email);
            
            return createUser;
        }

        public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            await _updateUserValidator.ValidateAndThrowAsync(user, cancellationToken);
            var updatingResult = await _userRepository.UpdateUserAsync(user, cancellationToken);
            var countEntries = await _unitOfWork.SaveChangesAsync(cancellationToken);
            var updatedUser = await _userRepository.GetByIdAsync(user.Id, cancellationToken);
            
            _logger.LogInformation(
                "User updated, Id={Id}, LoginBeforeUpdate={LoginBeforeUpdate}, EmailBeforeUpdate={EmailBeforeUpdate} " 
                + "- LoginAfterUpdate{LoginAfterUpdate}, EmailAfterUpdate={EmailAfterUpdate}",
                user.Id, user.Login, user.Email, updatedUser?.Login, updatedUser?.Email);
            
            return updatingResult && countEntries > 0;
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            if (!await _userRepository.IsExistAsync(id, cancellationToken))
            {
                var ex = new ObjectNotFoundException($"Пользоваль с id:{id} не найден");
                _logger.LogError(ex, "User not exist, Id={Id}", id);
                throw ex;
            }
            if (await _bankAccountRepository.ExistByUserIdAsync(id, cancellationToken))
            {
                var ex = new ValidationException(
                    $"Невозможно удалить пользователя по id:{id}, так как у него имеются аккаунт(ы)");
                _logger.LogError(ex, "User have bank account, Id={Id}", id);
                throw ex;
            }

            var targetUser = await _userRepository.GetByIdAsync(id, cancellationToken);
            var isDelete = await _userRepository.DeleteUserAsync(id, cancellationToken);
            var countEntries = await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User deleted, Id={Id}, Login={Login}, Email={Email}", id, targetUser?.Login,
                targetUser?.Email);
            
            return isDelete && countEntries > 0;
        }
    }
}