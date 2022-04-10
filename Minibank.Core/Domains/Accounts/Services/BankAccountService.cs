using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Accounts.Validators;
using Minibank.Core.Domains.Dal;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Repositories;
using ValidationException = Minibank.Core.Exceptions.FriendlyExceptions.ValidationException;

namespace Minibank.Core.Domains.Accounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _accountRepository;
        private readonly IMoneyTransferRepository _moneyTransferRepository;
        private readonly ICurrencyConverter _currencyConverter;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateBankAccountValidator _createBankAccountValidator;
        private readonly CloseBankAccountValidator _closeBankAccountValidator;
        private readonly UpdateBankAccountValidator _updateBankAccountValidator;
        private readonly IValidator<MoneyTransfer> _moneyTransferValidator;
        private readonly IValidator<string> _idValidator;

        public BankAccountService(IBankAccountRepository accountRepository, ICurrencyConverter currencyConverter,
            IMoneyTransferRepository moneyTransferRepository, IUnitOfWork unitOfWork,
            CreateBankAccountValidator createBankAccountValidator, CloseBankAccountValidator closeBankAccountValidator,
            IValidator<MoneyTransfer> moneyTransferValidator, IValidator<string> idValidator, UpdateBankAccountValidator updateBankAccountValidator)
        {
            _accountRepository = accountRepository;
            _currencyConverter = currencyConverter;
            _moneyTransferRepository = moneyTransferRepository;
            _unitOfWork = unitOfWork;
            _createBankAccountValidator = createBankAccountValidator;
            _closeBankAccountValidator = closeBankAccountValidator;
            _moneyTransferValidator = moneyTransferValidator;
            _idValidator = idValidator;
            _updateBankAccountValidator = updateBankAccountValidator;
        }

        public async Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            return await _accountRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken)
        {
            return await _accountRepository.GetAllAccountsAsync(cancellationToken);
        }

        public async Task<BankAccount> CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            await _createBankAccountValidator.ValidateAndThrowAsync(bankAccount, cancellationToken);
            var createBankAccount = await _accountRepository.CreateAccountAsync(bankAccount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return createBankAccount;
        }

        public async Task<BankAccount> UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(bankAccount.Id, cancellationToken);
            await _updateBankAccountValidator.ValidateAndThrowAsync(bankAccount, cancellationToken);
            
            var updateBankAccount =  await _accountRepository.UpdateAccountAsync(bankAccount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return updateBankAccount;
        }
        
        public async Task<bool> CloseAccountAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
            await _closeBankAccountValidator.ValidateAndThrowAsync(account, cancellationToken);
            
            var isClose = await _accountRepository.CloseAccountAsync(id, cancellationToken);
            var countEntries = await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return isClose && countEntries > 0;
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

            return Math.Round(moneyTransfer.Amount * 0.02m, 2);
        }

        public async Task<MoneyTransfer> TransferAmountAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken)
        {
            await _moneyTransferValidator.ValidateAndThrowAsync(moneyTransfer, cancellationToken);
            var fromAccount = await _accountRepository.GetByIdAsync(moneyTransfer.FromBankAccountId, cancellationToken);
            var toAccount = await _accountRepository.GetByIdAsync(moneyTransfer.ToBankAccountId, cancellationToken);
            
            if (fromAccount.Balance < moneyTransfer.Amount)
            {
                throw new ValidationException("недостаточно средств для перевода");
            }
            
            fromAccount.Balance -= moneyTransfer.Amount;
            moneyTransfer.Amount -= await CalculateCommissionAsync(moneyTransfer, cancellationToken);
            var conversionAmount =
                Math.Round(
                    await _currencyConverter.ConvertAsync(moneyTransfer.Amount, fromAccount.Currency, toAccount.Currency, cancellationToken),
                    2);
            toAccount.Balance += conversionAmount;
            
            var createMoneyTransfer = await _moneyTransferRepository.CreateTransferAsync(moneyTransfer, cancellationToken);
            await _accountRepository.UpdateAccountAsync(fromAccount, cancellationToken);
            await _accountRepository.UpdateAccountAsync(toAccount, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return createMoneyTransfer;
        }
    }
}