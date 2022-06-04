using System;
using System.Data;
using System.Linq;
using FluentValidation;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Accounts.Validators;

namespace Minibank.Core.Domains.Transfers.Validators
{
    public class MoneyTransferValidator : AbstractValidator<MoneyTransfer>
    {
        public MoneyTransferValidator(IBankAccountRepository bankAccountRepository)
        {
            RuleFor(transfer => transfer.FromBankAccountId).Cascade(CascadeMode.Stop).NotEmpty().WithMessage(
                "id аккаунта отправителя не должен быть пустым").MustExist(bankAccountRepository).MustOpen(
                    bankAccountRepository).DependentRules(
                        () => RuleFor(transfer => transfer.ToBankAccountId).Cascade(CascadeMode.Stop).NotEmpty()
                            .WithMessage("id аккаунта получателя не должен быть пустым").MustExist(
                                bankAccountRepository).MustOpen(bankAccountRepository));

            RuleFor(transfer => transfer).Must((transfer, _) => transfer.FromBankAccountId != transfer.ToBankAccountId)
                .WithMessage(transfer => $"невозможно совершить перевод между одним и тем же аккаунтом, id: {transfer.FromBankAccountId}");

            RuleFor(transfer => transfer.Amount).GreaterThan(0).WithMessage("Сумма первода должна быть больше, чем ноль");

            RuleFor(transfer => transfer.Currency).IsInEnum().WithMessage(
                "Неверный тип валюты. Допускаются следующие типы: " +
                string.Join("; ", Enum.GetNames<Currency>().Select(enumName => $"{enumName} - {(int)Enum.Parse<Currency>(enumName)}")));
        }
    }
}