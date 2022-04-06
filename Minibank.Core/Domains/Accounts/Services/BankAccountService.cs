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
        private readonly IValidator<MoneyTransfer> _moneyTransferValidator;

        public BankAccountService(IBankAccountRepository accountRepository, ICurrencyConverter currencyConverter,
            IMoneyTransferRepository moneyTransferRepository, IUnitOfWork unitOfWork,
            CreateBankAccountValidator createBankAccountValidator, CloseBankAccountValidator closeBankAccountValidator,
            IValidator<MoneyTransfer> moneyTransferValidator)
        {
            _accountRepository = accountRepository;
            _currencyConverter = currencyConverter;
            _moneyTransferRepository = moneyTransferRepository;
            _unitOfWork = unitOfWork;
            _createBankAccountValidator = createBankAccountValidator;
            _closeBankAccountValidator = closeBankAccountValidator;
            _moneyTransferValidator = moneyTransferValidator;
        }

        public Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("id не должне быть пустым");
            }
            
            return _accountRepository.GetByIdAsync(id, cancellationToken);
        }

        public Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken)
        {
            return _accountRepository.GetAllAccountsAsync(cancellationToken);
        }

        public async Task CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            await _createBankAccountValidator.ValidateAndThrowAsync(bankAccount, cancellationToken);
            await _accountRepository.CreateAccountAsync(bankAccount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(bankAccount.Id))
            {
                throw new ValidationException("id не должне быть пустым");
            }
            
            var targetBankAccount = await _accountRepository.GetByIdAsync(bankAccount.Id, cancellationToken);
            if (!targetBankAccount.IsOpen)
            {
                throw new ValidationException($"акканут по id: {targetBankAccount.Id}, закрыт, изменения невозможны");
            }
            
            await _accountRepository.UpdateAccountAsync(bankAccount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        
        public async Task CloseAccountAsync(string id, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("id не должне быть пустым");
            }
            
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
            await _closeBankAccountValidator.ValidateAndThrowAsync(account, cancellationToken);
            
            await _accountRepository.CloseAccountAsync(id, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task<double> CalculateCommissionAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken)
        {
            await _moneyTransferValidator.ValidateAndThrowAsync(moneyTransfer, cancellationToken);
            var fromAccount = await _accountRepository.GetByIdAsync(moneyTransfer.FromBankAccountId, cancellationToken);
            var toAccount = await _accountRepository.GetByIdAsync(moneyTransfer.ToBankAccountId, cancellationToken);

            if (fromAccount.UserId == toAccount.UserId)
            {
                return 0;
            }

            return Math.Round(moneyTransfer.Amount * 0.02, 2);
        }

        public async Task TransferAmountAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken)
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
            
            await _moneyTransferRepository.CreateTransferAsync(moneyTransfer, cancellationToken);
            await _accountRepository.UpdateAccountAsync(fromAccount, cancellationToken);
            await _accountRepository.UpdateAccountAsync(toAccount, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}