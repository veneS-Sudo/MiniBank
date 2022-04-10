using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;

namespace Minibank.Core.Domains.Users.Validators
{
    public class DeleteUserValidator : AbstractValidator<string>
    {
        public DeleteUserValidator(IBankAccountRepository accountRepository)
        {
            RuleFor(id => id).NotEmpty().WithMessage("id не должен быть пустым");
            RuleFor(id => id).MustAsync( async (id, token) => !(await accountRepository.ExistsByUserIdAsync(id, token)))
                .WithMessage(id => $"Невозможно удалить пользователя по id:{id}, так как у него есть аккаунт(ы)");
        }
    }
}