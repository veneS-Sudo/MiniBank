using FluentValidation;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Accounts.Validators
{
    public class UpdateBankAccountValidator : AbstractValidator<BankAccount>
    {
        public UpdateBankAccountValidator(IUserRepository userRepository)
        {
            RuleFor(account => account.UserId).MustAsync(userRepository.ExistsAsync).WithMessage(
                account => $"не существует пользователя с id:{account.UserId}");
            RuleFor(account => account.IsOpen).Equal(true).WithMessage(
                account => $"акканут по id: {account.Id}, закрыт, изменения невозможны");
        }
    }
}