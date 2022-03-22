using System;
using System.Collections.Generic;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Repositories;
using Minibank.Core.Domains.Users;
using Minibank.Core.Domains.Users.Repositories;
using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Core.Domains.Accounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly ITransferRepository _transferRepository;
        private readonly ICurrencyConverter _currencyConverter;

        public BankAccountService(IBankAccountRepository accountRepository,
            IUserRepository userRepository, ICurrencyConverter currencyConverter, ITransferRepository transferRepository)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _currencyConverter = currencyConverter;
            _transferRepository = transferRepository;
        }

        public BankAccount GetById(string id)
        {
            return _accountRepository.GetById(id);
        }

        public List<BankAccount> GetAllAccounts()
        {
            return _accountRepository.GetAllAccounts();
        }

        public void CreateAccount(string id, Currency currency)
        {
            if (_userRepository.Exists(id))
            {
                throw new ValidationException($"Невозможно создать аккунт для пользователя c id:{id}");
            }
            
            _accountRepository.CreateAccount(id, currency);
        }

        public void UpdateAccount(BankAccount account)
        {
            _accountRepository.UpdateAccount(account);
        }
        
        public void CloseAccount(string id)
        {
            var account = _accountRepository.GetById(id);
            if (!account.IsOpen)
            {
                throw new ValidationException($"Аккаунт по id:{id}, уже закрыт!");
            }
            
            if (account.Balance != 0)
            {
                throw new ValidationException(
                    $"У аккаунта по id:{id} баланс не равен нулю. При удалении баланс на счету должен быть нулевым!");
            }
            
            _accountRepository.CloseAccount(id);            
        }

        public double CalculateCommission(Transfer transfer)
        {
            var fromAccount = _accountRepository.GetById(transfer.FromAccountId);
            var toAccount = _accountRepository.GetById(transfer.ToAccountId);

            if (fromAccount.UserId == toAccount.UserId)
            {
                return 0;
            }

            return Math.Round(transfer.Amount * 0.02, 2);
        }

        public void TransferAmount(Transfer transfer)
        {
            var fromAccount = _accountRepository.GetById(transfer.FromAccountId);
            var toAccount = _accountRepository.GetById(transfer.ToAccountId);

            if (fromAccount.Balance < transfer.Amount)
            {
                throw new ValidationException("Недостаточно средств для перевода!");
            }
            
            fromAccount.Balance -= transfer.Amount;
            var conversionAmount = transfer.Amount;
            if (fromAccount.Currency != toAccount.Currency)
            {
                conversionAmount = _currencyConverter.Convert(transfer.Amount, fromAccount.Currency, toAccount.Currency);
            }
            transfer.Amount -= CalculateCommission(transfer);
            toAccount.Balance += conversionAmount;
            
            _transferRepository.CreateTransfer(transfer);
        }
    }
}