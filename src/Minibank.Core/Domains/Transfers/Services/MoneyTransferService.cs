using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<MoneyTransferService> _logger;

        public MoneyTransferService(IMoneyTransferRepository moneyTransferRepository, ICurrencyConverter currencyConverter,
            IValidator<MoneyTransfer> moneyTransferValidator, IBankAccountRepository accountRepository, IUnitOfWork unitOfWork,
            ICommissionCalculator commissionCalculator, ILogger<MoneyTransferService> logger)
        {
            _moneyTransferRepository = moneyTransferRepository;
            _currencyConverter = currencyConverter;
            _moneyTransferValidator = moneyTransferValidator;
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _commissionCalculator = commissionCalculator;
            _logger = logger;
        }

        public async Task<List<MoneyTransfer>> GetAllTransfersAsync(string bankAccountId, CancellationToken cancellationToken)
        {
            var accountExist = await _accountRepository.IsExistAsync(bankAccountId, cancellationToken);
            if (!accountExist)
            {
                throw new ObjectNotFoundException($"Аккаунт с id: {bankAccountId} не найден");
            }
            
            return await _moneyTransferRepository.GetAllTransfersAsync(bankAccountId, cancellationToken);
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
            // transfer sum calculating
            fromAccount.Balance -= moneyTransfer.Amount;
            var commission =await _commissionCalculator.CalculateCommissionAsync(moneyTransfer, cancellationToken);
            moneyTransfer.Amount -= commission;
            var conversionAmount =
                Math.Round(
                    await _currencyConverter.ConvertAsync(moneyTransfer.Amount, fromAccount.Currency, toAccount.Currency, cancellationToken),
                    2);
            toAccount.Balance += conversionAmount;
            
            var transferId = await _moneyTransferRepository.CreateTransferAsync(moneyTransfer, cancellationToken);
            
            _logger.LogInformation(
                "Money transfer created, Id={Id}, Amount={Amount}, Currency={Currency}," + 
                " FromBankAccount={FromBankAccount}, ToBankAccount={ToBankAccount}, Commission={Commission}",
                transferId, moneyTransfer.Amount, moneyTransfer.Currency.ToString(),
                moneyTransfer.FromBankAccountId, moneyTransfer.ToBankAccountId, commission);
            
            var updatingResultFrom = await _accountRepository.UpdateAccountAsync(fromAccount, cancellationToken);
            var updatingResultTo = await _accountRepository.UpdateAccountAsync(toAccount, cancellationToken);
            if (!updatingResultFrom || !updatingResultTo)
            {
                string accountIds = (updatingResultFrom ? string.Empty : fromAccount.Id) +
                                    (updatingResultFrom ? string.Empty : toAccount.Id);
                throw new TransferNotCompletedException(
                    $"Операция перевода не была выполнена вследствии неудачной попытки обновления аккаунта(ов) с id:" +
                    accountIds);
            }
            
            var updatedFromAccount = await _accountRepository.GetByIdAsync(moneyTransfer.FromBankAccountId, cancellationToken);
            var updatedToAccount = await _accountRepository.GetByIdAsync(moneyTransfer.ToBankAccountId, cancellationToken);
            //save bank accounts
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            _logger.LogInformation(
                "Accounts of transfer updated, FromId={FromBankAccountId}: BalanceBeforeTransfer={FromBalanceBefore}, BalanceAfterTransfer{FromBalanceAfter};" +
                "ToId={ToBankAccountId}: BalanceBeforeTransfer={ToBalanceBefore}, BalanceAfterTransfer{ToBalanceAfter}",
                fromAccount.Id, fromAccount.Balance, updatedFromAccount?.Balance, toAccount.Id, toAccount.Balance,
                updatedToAccount?.Balance);
            return transferId;
        }
    }
}