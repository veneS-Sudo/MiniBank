using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;

namespace Minibank.Core.Domains.Transfers.Validators
{
    public class MoneyTransferValidator : AbstractValidator<MoneyTransfer>
    {
        public MoneyTransferValidator(IBankAccountRepository bankAccountRepository)
        {
            RuleFor(transfer => transfer.FromBankAccountId).NotEmpty().WithMessage(
                "id отправителя при переводе не должно быть пустым");
            
            RuleFor(transfer => transfer).Must((transfer, _) => transfer.FromBankAccountId != transfer.ToBankAccountId)
                .WithMessage(transfer => $"не возможно совершить перевод между одним и тем же аккаунтом, id: {transfer.FromBankAccountId}");
            
            RuleFor(transfer => transfer.Amount).GreaterThan(0).WithMessage("Сумма первода должна быть положительной");
            
            RuleFor(transfer => transfer.FromBankAccountId).MustAsync(bankAccountRepository.BankAccountIsOpenAsync)
                .WithMessage(transfer => $"акканут отправителя по id: {transfer.FromBankAccountId} закрыт, перевод и вычисление комиссии невозможен");
            
            RuleFor(transfer => transfer.ToBankAccountId).NotEmpty().WithMessage(
                "id получателя при переводе не должно быть пустым");
            
            RuleFor(transfer => transfer.ToBankAccountId).MustAsync(bankAccountRepository.BankAccountIsOpenAsync)
                .WithMessage(transfer => $"акканут получателя по id: {transfer.ToBankAccountId} закрыт, перевод и вычисление комиссии невозможен");
        }
    }
}