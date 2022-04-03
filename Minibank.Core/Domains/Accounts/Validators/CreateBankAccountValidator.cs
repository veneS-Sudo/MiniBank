using System.Data;
using FluentValidation;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Accounts.Validators
{
    public class CreateBankAccountValidator : AbstractValidator<BankAccount>
    {
        public CreateBankAccountValidator(IUserRepository userRepository)
        {
            RuleFor(account => account.Balance).GreaterThanOrEqualTo(0).WithMessage(
                "невозможно создать аккаунт с отрицательным балансом");
            RuleFor(account => account.UserId).NotEmpty().WithMessage(
                "id пользователя, для которого требуется создать банковский, не должен быть пустым");
            RuleFor(account => account.UserId)
                .MustAsync((userId, _) => userRepository.ExistsAsync(userId))
                .WithMessage(account =>  $"невозможно создать аккунт для пользователя c id:{account.UserId}");
            RuleFor(account => account.Currency).IsInEnum().WithMessage("неверный тип валюты");
        }
    }
}