using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Transfers.Repositories;
using Minibank.Core.Exceptions.FriendlyExceptions;

namespace Minibank.Core.Domains.Transfers.Services
{
    public class MoneyTransferService : IMoneyTransferService
    {
        private readonly IMoneyTransferRepository _moneyTransferRepository;
        private readonly IBankAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly ICommissionCalculator _commissionCalculator;
        private readonly IValidator<MoneyTransfer> _moneyTransferValidator;

        public MoneyTransferService(IMoneyTransferRepository moneyTransferRepository, ICurrencyConverter currencyConverter,
            IValidator<MoneyTransfer> moneyTransferValidator, IBankAccountRepository accountRepository, IUnitOfWork unitOfWork,
            ICommissionCalculator commissionCalculator)
        {
            _moneyTransferRepository = moneyTransferRepository;
            _currencyConverter = currencyConverter;
            _moneyTransferValidator = moneyTransferValidator;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _commissionCalculator = commissionCalculator;
        }

        public async Task<string> TransferAmountAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken)
        {
            await _moneyTransferValidator.ValidateAndThrowAsync(moneyTransfer, cancellationToken);
            var fromAccount = await _accountRepository.GetByIdAsync(moneyTransfer.FromBankAccountId, cancellationToken);
            var toAccount = await _accountRepository.GetByIdAsync(moneyTransfer.ToBankAccountId, cancellationToken);
            
            if (fromAccount.Balance < moneyTransfer.Amount)
            {
                throw new LackOfFundsException($"у аккаунта по id: {fromAccount.Id}, недостаточно средств для перевода");
            }
            
            fromAccount.Balance -= moneyTransfer.Amount;
            moneyTransfer.Amount -= await _commissionCalculator.CalculateCommissionAsync(moneyTransfer, cancellationToken);
            var conversionAmount =
                Math.Round(
                    await _currencyConverter.ConvertAsync(moneyTransfer.Amount, fromAccount.Currency, toAccount.Currency, cancellationToken),
                    2);
            toAccount.Balance += conversionAmount;
            
            var transferId = await _moneyTransferRepository.CreateTransferAsync(moneyTransfer, cancellationToken);
            await _accountRepository.UpdateAccountAsync(fromAccount, cancellationToken);
            await _accountRepository.UpdateAccountAsync(toAccount, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return transferId;
        }
    }
}