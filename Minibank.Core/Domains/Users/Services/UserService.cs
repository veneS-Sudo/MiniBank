using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Domains.Users.Validators;
using Minibank.Core.UniversalValidators;

namespace Minibank.Core.Domains.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IValidator<User> _userValidator;
        private readonly IdEntityValidator _idValidator;
        private readonly DeleteUserValidator _deleteUserValidator;

        public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork, IValidator<User> userValidator,
            IdEntityValidator idValidator, DeleteUserValidator deleteUserValidator)
        {
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
            _userValidator = userValidator;
            _idValidator = idValidator;
            _deleteUserValidator = deleteUserValidator;
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
            await _userValidator.ValidateAndThrowAsync(user, cancellationToken);
            var createUser = await _userRepository.CreateUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return createUser;
        }

        public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(user.Id, cancellationToken);
            await _userValidator.ValidateAndThrowAsync(user, cancellationToken);
            var updateUser = await _userRepository.UpdateUserAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return updateUser;
        }

        public async Task<bool> DeleteUserAsync(string id, CancellationToken cancellationToken)
        {
            await _deleteUserValidator.ValidateAndThrowAsync(id, cancellationToken);
           
            var isDelete = await _userRepository.DeleteUserAsync(id, cancellationToken);
            var countEntries = await _unitOfWork.SaveChangesAsync(cancellationToken);
            return isDelete && countEntries > 0;
        }
    }
}