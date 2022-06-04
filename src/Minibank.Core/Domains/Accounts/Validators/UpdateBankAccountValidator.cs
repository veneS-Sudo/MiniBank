using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;

namespace Minibank.Core.Domains.Accounts.Validators
{
    public class UpdateBankAccountValidator : AbstractValidator<BankAccount>
    {
        public UpdateBankAccountValidator(IBankAccountRepository bankAccountRepository)
        {
            RuleFor(account => account.Id).Cascade(CascadeMode.Stop).MustExist(bankAccountRepository)
                .MustOpen(bankAccountRepository);
        }
    }
}