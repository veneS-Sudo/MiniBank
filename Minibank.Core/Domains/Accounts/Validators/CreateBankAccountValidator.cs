using System;
using System.Data;
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
            RuleFor(account => account.UserId).NotEmpty().WithMessage(
                "id пользователя, для которого требуется создать банковский, не должен быть пустым");
            RuleFor(account => account.UserId)
                .MustAsync(userRepository.ExistsAsync)
                .WithMessage(account =>  $"невозможно создать аккунт для пользователя c id:{account.UserId}, поскольку такового не существует");
            
            RuleFor(account => account.Currency).IsInEnum()
                .WithMessage("Неверный тип валюты. Допускаются следующие типы: " +
            string.Join("; ", Enum.GetNames<Currency>().Select(enumName => $"{enumName} - {(int)Enum.Parse<Currency>(enumName)}")));
        }
    }
}