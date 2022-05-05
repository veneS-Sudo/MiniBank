using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;

namespace Minibank.Core.Domains.Users.Validators
{
    public class DeleteUserValidator : AbstractValidator<string>
    {
        public DeleteUserValidator(IBankAccountRepository accountRepository)
        {
            RuleFor(id => id).Cascade(CascadeMode.Stop).NotEmpty().WithMessage("id не должен быть пустым")
                .MustAsync(async (id, token) => !(await accountRepository.ExistsByUserIdAsync(id, token)))
                .WithMessage(id => $"Невозможно удалить пользователя по id:{id}, так как у него есть аккаунт(ы)");
        }
    }
}