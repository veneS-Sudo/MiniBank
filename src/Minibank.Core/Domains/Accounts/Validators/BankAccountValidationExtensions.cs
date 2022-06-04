using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;

namespace Minibank.Core.Domains.Accounts.Validators
{
    public static class BankAccountValidationExtensions
    {
        public static IRuleBuilderOptions<T, string> MustOpen<T>(
            this IRuleBuilder<T, string> ruleBuilder, IBankAccountRepository bankAccountRepository)
        {
            return ruleBuilder.MustAsync(bankAccountRepository.IsOpenAsync).WithMessage((_, property) =>
                    $"акканут по id: {property} закрыт, любые действия с ним невозможны");
        }

        public static IRuleBuilderOptions<T, string> MustExist<T>(this IRuleBuilder<T, string> ruleBuilder,
            IBankAccountRepository bankAccountRepository)
        {
            return ruleBuilder.MustAsync(bankAccountRepository.IsExistAsync).WithMessage(
                    (_, property) => $"аккаунт по id: {property} не найден");
        }
    }
}