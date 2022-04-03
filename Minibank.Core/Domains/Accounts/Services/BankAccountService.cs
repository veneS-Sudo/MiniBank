using System;
using System.Collections.Generic;
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

        public Task<BankAccount> GetByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("id не должне быть пустым");
            }
            
            return _accountRepository.GetByIdAsync(id);
        }

        public Task<List<BankAccount>> GetAllAccountsAsync()
        {
            return _accountRepository.GetAllAccountsAsync();
        }

        public async Task CreateAccountAsync(BankAccount bankAccount)
        {
            await _createBankAccountValidator.ValidateAndThrowAsync(bankAccount);
            await _accountRepository.CreateAccountAsync(bankAccount);
            _unitOfWork.SaveChanges();
        }

        public async Task UpdateAccountAsync(BankAccount bankAccount)
        {
            if (string.IsNullOrEmpty(bankAccount.Id))
            {
                throw new ValidationException("id не должне быть пустым");
            }
            
            var targetBankAccount = await _accountRepository.GetByIdAsync(bankAccount.Id);
            if (!targetBankAccount.IsOpen)
            {
                throw new ValidationException($"акканут по id: {targetBankAccount.Id}, закрыт, изменения невозможны");
            }
            
            await _accountRepository.UpdateAccountAsync(bankAccount);
            _unitOfWork.SaveChanges();
        }
        
        public async Task CloseAccountAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ValidationException("id не должне быть пустым");
            }
            
            var account = await _accountRepository.GetByIdAsync(id);
            await _closeBankAccountValidator.ValidateAndThrowAsync(account);
            
            await _accountRepository.CloseAccountAsync(id);
            _unitOfWork.SaveChanges();
        }

        public async Task<double> CalculateCommissionAsync(MoneyTransfer moneyTransfer)
        {
            await _moneyTransferValidator.ValidateAndThrowAsync(moneyTransfer);
            var fromAccount = await _accountRepository.GetByIdAsync(moneyTransfer.FromBankAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(moneyTransfer.ToBankAccountId);

            if (fromAccount.UserId == toAccount.UserId)
            {
                return 0;
            }

            return Math.Round(moneyTransfer.Amount * 0.02, 2);
        }

        public async Task TransferAmountAsync(MoneyTransfer moneyTransfer)
        {
            await _moneyTransferValidator.ValidateAndThrowAsync(moneyTransfer);
            
            var fromAccount = await _accountRepository.GetByIdAsync(moneyTransfer.FromBankAccountId);
            var toAccount = await _accountRepository.GetByIdAsync(moneyTransfer.ToBankAccountId);
            
            if (fromAccount.Balance < moneyTransfer.Amount)
            {
                throw new ValidationException("недостаточно средств для перевода");
            }
            
            fromAccount.Balance -= moneyTransfer.Amount;
            moneyTransfer.Amount -= await CalculateCommissionAsync(moneyTransfer);
            var conversionAmount =
                Math.Round(
                    await _currencyConverter.ConvertAsync(moneyTransfer.Amount, fromAccount.Currency, toAccount.Currency),
                    2);
            toAccount.Balance += conversionAmount;
            
            await _moneyTransferRepository.CreateTransferAsync(moneyTransfer);
            await _accountRepository.UpdateAccountAsync(fromAccount);
            await _accountRepository.UpdateAccountAsync(toAccount);
            
            _unitOfWork.SaveChanges();
        }
    }
}