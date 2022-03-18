using System.Collections.Generic;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IBankAccountRepository _accountRepository;

        public UserService(IUserRepository userRepository, IBankAccountRepository accountRepository)
        {
            _userRepository = userRepository;
            _accountRepository = accountRepository;
        }
        
        public User GetById(string id)
        {
            return _userRepository.GetById(id);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _userRepository.GetAllUsers();
        }

        public void CreateUser(User user)
        {
            _userRepository.CreateUser(user);
        }

        public void UpdateUser(User user)
        {
            _userRepository.UpdateUser(user);
        }

        public void DeleteUser(string id)
        {
            if (!_accountRepository.ExistsByUserId(id))
            {
                throw new ValidationException("Невозможно удалить пользователя с существущим аккаунтом.");
            }
            
            _userRepository.DeleteUser(id);
        }
    }
}