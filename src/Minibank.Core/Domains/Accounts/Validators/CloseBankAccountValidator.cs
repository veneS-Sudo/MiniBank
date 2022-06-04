using FluentValidation;

namespace Minibank.Core.Domains.Accounts.Validators
{
    public class CloseBankAccountValidator : AbstractValidator<BankAccount>
    {
        public CloseBankAccountValidator()
        {
            RuleFor(account => account.IsOpen).Equal(true).WithMessage(
                account => $"Акканут по id: {account.Id}, уже закрыт, изменения невозможны");
            RuleFor(account => account.Balance).Equal(0).WithMessage( account =>
                $"У аккаунта по id:{account.Id} баланс не равен нулю. При закрытии баланс на счету должен быть нулевым");
        }
    }
}