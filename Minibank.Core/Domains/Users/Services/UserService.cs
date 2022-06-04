using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
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

        public UserService(IUserRepository userRepository, IBankAccountRepository bankAccountRepository, IUnitOfWork unitOfWork, IdEntityValidator idValidator,
            CreateUserValidator createUserValidator, UpdateUserValidator updateUserValidator)
        {
            _userRepository = userRepository;
            _bankAccountRepository = bankAccountRepository;
            _unitOfWork = unitOfWork;
            _idValidator = idValidator;
            _createUserValidator = createUserValidator;
            _updateUserValidator = updateUserValidator;
        }
        
        public async Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            return await _userRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return await _userRepository.GetAllUsersAsync(cancellationToken);
        }

        public async Task<User> CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            await _createUserValidator.ValidateAndThrowAsync(user, cancellationToken);
            var createUser = await _userRepository.CreateUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return createUser;
        }

        public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            await _updateUserValidator.ValidateAndThrowAsync(user, cancellationToken);
            var updatingResult = await _userRepository.UpdateUserAsync(user, cancellationToken);
            var countEntries = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return updatingResult && countEntries > 0;
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            if (!await _userRepository.IsExistAsync(id, cancellationToken))
            {
                throw new ObjectNotFoundException($"Пользоваль с id:{id} не найден");
            }
            if (await _bankAccountRepository.ExistByUserIdAsync(id, cancellationToken))
            {
                throw new ValidationException(
                    $"Невозможно удалить пользователя по id:{id}, так как у него имеются аккаунт(ы)");
            }
            
            var isDelete = await _userRepository.DeleteUserAsync(id, cancellationToken);
            var countEntries = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return isDelete && countEntries > 0;
        }
    }
}