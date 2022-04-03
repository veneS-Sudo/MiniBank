using System.Collections.Generic;
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
        
        public Task<User> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("Id не должен быть пустым");
            }
            
            return _userRepository.GetByIdAsync(id);
        }

        public Task<List<User>> GetAllUsersAsync()
        {
            return _userRepository.GetAllUsersAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _userValidator.ValidateAndThrowAsync(user);
            await _userRepository.CreateUserAsync(user);
            _unitOfWork.SaveChanges();
        }

        public async Task UpdateUserAsync(User user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                throw new ValidationException("Для обновления данных пользователя необходим непустой id");
            }
            
            await _userValidator.ValidateAndThrowAsync(user);
            await _userRepository.UpdateUserAsync(user);
            _unitOfWork.SaveChanges();
        }

        public async Task DeleteUserAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("Id не должен быть пустым");
            }
            if (await _accountRepository.ExistsByUserIdAsync(id))
            {
                throw new ValidationException($"Невозможно удалить пользователя по id:{id}, так как у него есть аккаунт(ы)");
            }
            
            await _userRepository.DeleteUserAsync(id);
            _unitOfWork.SaveChanges();
        }
    }
}