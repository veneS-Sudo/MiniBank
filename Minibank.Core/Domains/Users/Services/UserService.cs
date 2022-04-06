using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Users.Repositories;
using ValidationException = Minibank.Core.Exceptions.FriendlyExceptions.ValidationException;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<User> _userValidator;

        public UserService(IUserRepository userRepository, IBankAccountRepository accountRepository,
            IUnitOfWork unitOfWork, IValidator<User> userValidator)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _userValidator = userValidator;
        }
        
        public Task<User> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("Id не должен быть пустым");
            }
            
            return _userRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
        {
            return _userRepository.GetAllUsersAsync(cancellationToken);
        }

        public async Task CreateUserAsync(User user, CancellationToken cancellationToken)
        {
            await _userValidator.ValidateAndThrowAsync(user, cancellationToken);
            await _userRepository.CreateUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                throw new ValidationException("Для обновления данных пользователя необходим непустой id");
            }
            
            await _userValidator.ValidateAndThrowAsync(user, cancellationToken);
            await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task DeleteUserAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("Id не должен быть пустым");
            }
            if (await _accountRepository.ExistsByUserIdAsync(id, cancellationToken))
            {
                throw new ValidationException($"Невозможно удалить пользователя по id:{id}, так как у него есть аккаунт(ы)");
            }
            
            await _userRepository.DeleteUserAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}