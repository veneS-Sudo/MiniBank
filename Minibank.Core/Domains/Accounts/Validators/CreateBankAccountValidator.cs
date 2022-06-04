using System;
using System.Linq;
using FluentValidation;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Users.Repositories;

namespace Minibank.Core.Domains.Accounts.Validators
{
    public class CreateBankAccountValidator : AbstractValidator<BankAccount>
    {
        public CreateBankAccountValidator(IUserRepository userRepository)
        {
            RuleFor(account => account.Balance).GreaterThanOrEqualTo(0).WithMessage(
                "невозможно создать аккаунт с отрицательным балансом");

            RuleFor(account => account.UserId).MustAsync(userRepository.IsExistAsync)
                .WithMessage(user => $"Пользователь с id:{user.Id}, не найден");
            
            RuleFor(account => account.Currency).IsInEnum()
                .WithMessage("Неверный тип валюты. Допускаются следующие типы: " +
            string.Join("; ", Enum.GetNames<Currency>().Select(enumName => $"{enumName} - {(int)Enum.Parse<Currency>(enumName)}")));
        }
    }
}