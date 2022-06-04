using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Domains.Accounts.Repositories;

namespace Minibank.Core.Domains.Transfers.Services
{
    public class CommissionCalculator : ICommissionCalculator
    {
        private readonly IBankAccountRepository _accountRepository;
        private readonly IValidator<MoneyTransfer> _moneyTransferValidator;
        private readonly IFractionalNumberEditor _fractionalNumberEditor;

        public CommissionCalculator(IBankAccountRepository accountRepository, IValidator<MoneyTransfer> moneyTransferValidator,
            IFractionalNumberEditor fractionalNumberEditor)
        {
            _accountRepository = accountRepository;
            _moneyTransferValidator = moneyTransferValidator;
            _fractionalNumberEditor = fractionalNumberEditor;
        }

        public async Task<decimal> CalculateCommissionAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken)
        {
            await _moneyTransferValidator.ValidateAndThrowAsync(moneyTransfer, cancellationToken);
            var fromAccount = await _accountRepository.GetByIdAsync(moneyTransfer.FromBankAccountId, cancellationToken);
            var toAccount = await _accountRepository.GetByIdAsync(moneyTransfer.ToBankAccountId, cancellationToken);
            
            if (fromAccount.UserId == toAccount.UserId)
            {
                return 0m;
            }
            
            return _fractionalNumberEditor.Round(moneyTransfer.Amount * 0.02m, 2);
        }
    }
}