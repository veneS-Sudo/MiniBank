using FluentValidation;
using Minibank.Core.Domains.Accounts.Validators;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Users.Validators
{
    public class UpdateUserValidator : AbstractValidator<User>
    {
        public UpdateUserValidator(IUserRepository userRepository)
        {
            RuleFor(user => user.Id).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("id не должен быть пустым")
                .MustAsync(userRepository.IsExistAsync).WithMessage(user => $"Пользователь с id:{user.Id}, не найден");
            Include(new CreateUserValidator());
        }
    }
}