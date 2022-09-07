using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Accounts.Validators;
using Minibank.Core.Domains.Dal;
using Minibank.Core.UniversalValidators;    

namespace Minibank.Core.Domains.Accounts.Services
{
    public class BankAccountService : IBankAccountService
    {
        private readonly IBankAccountRepository _accountRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly CreateBankAccountValidator _createBankAccountValidator;
        private readonly CloseBankAccountValidator _closeBankAccountValidator;
        private readonly UpdateBankAccountValidator _updateBankAccountValidator;
        private readonly ILogger<BankAccountService> _logger;
        private readonly IdEntityValidator _idValidator;

        public BankAccountService(IBankAccountRepository accountRepository, IUnitOfWork unitOfWork,
            CreateBankAccountValidator createBankAccountValidator, CloseBankAccountValidator closeBankAccountValidator,
            IdEntityValidator idValidator, UpdateBankAccountValidator updateBankAccountValidator, ILogger<BankAccountService> logger)
        {
            _accountRepository = accountRepository;
            _unitOfWork = unitOfWork;
            _createBankAccountValidator = createBankAccountValidator;
            _closeBankAccountValidator = closeBankAccountValidator;
            _idValidator = idValidator;
            _updateBankAccountValidator = updateBankAccountValidator;
            _logger = logger;
        }

        public async Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            var bankAccount = await _accountRepository.GetByIdAsync(id, cancellationToken);

            _logger.LogInformation("Bank account received, Id={Id}, UserId{UserId}, Balance={Balance}," + 
                " Currency={Currency}, IsOpen={IsOpen}, DataOpen={Open}, DataClose={Close}",
                bankAccount?.Id, bankAccount?.UserId, bankAccount?.Balance, bankAccount?.Currency.ToString(),
                bankAccount?.IsOpen, bankAccount?.DateOpen, bankAccount?.DateClose);
            
            return bankAccount;
        }

        public async Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken)
        {
            var bankAccounts = await _accountRepository.GetAllAccountsAsync(cancellationToken);
            
            _logger.LogInformation("Bank accounts received, Count={Count}", bankAccounts?.Count);

            return bankAccounts;
        }

        public async Task<BankAccount> CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            await _createBankAccountValidator.ValidateAndThrowAsync(bankAccount, cancellationToken);
            var createBankAccount = await _accountRepository.CreateAccountAsync(bankAccount, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Bank account created, Id={Id}, UserId{UserId}, Balance={Balance}," + 
                " Currency={Currency}, IsOpen={IsOpen}, DataOpen={Open}, DataClose={Close}",
                bankAccount?.Id, bankAccount?.UserId, bankAccount?.Balance, bankAccount?.Currency.ToString(),
                bankAccount?.IsOpen, bankAccount?.DateOpen, bankAccount?.DateClose);
            
            return createBankAccount;
        }

        public async Task<bool> UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(bankAccount.Id, cancellationToken);
            await _updateBankAccountValidator.ValidateAndThrowAsync(bankAccount, cancellationToken);
            var updatingResult =  await _accountRepository.UpdateAccountAsync(bankAccount, cancellationToken);
            var savingResult = await _unitOfWork.SaveChangesAsync(cancellationToken);
            var updatedBankAccount = await _accountRepository.GetByIdAsync(bankAccount.Id, cancellationToken);

            _logger.LogInformation("Bank account updated, Id={Id}, UserId{UserId}, BalanceBeforeUpdate={BalanceBeforeUpdate},"+
                " BalanceAfterUpdate={BalanceAfterUpdate}, Currency={Currency}, IsOpen={IsOpen}, DataOpen={Open}, DataClose={Close}",
                bankAccount.Id, bankAccount.UserId, bankAccount.Balance, updatedBankAccount?.Balance,
                bankAccount.Currency.ToString(), bankAccount.IsOpen, bankAccount.DateOpen, bankAccount.DateClose);
            
            return updatingResult && savingResult > 0;
        }
        
        public async Task<bool> CloseAccountAsync(string id, CancellationToken cancellationToken)
        {
            await _idValidator.ValidateAndThrowAsync(id, cancellationToken);
            
            var account = await _accountRepository.GetByIdAsync(id, cancellationToken);
            await _closeBankAccountValidator.ValidateAndThrowAsync(account, cancellationToken);
            
            var isClose = await _accountRepository.CloseAccountAsync(id, cancellationToken);
            var countEntries = await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Bank account created, Id={Id}, UserId{UserId}, Balance={Balance}"
                                   + ", Currency={Currency}, IsOpen={IsOpen}, DataOpen={Open}, DataClose={Close}",
                account?.Id, account?.UserId, account?.Balance, account?.Currency.ToString(),
                account?.IsOpen, account?.DateOpen, account?.DateClose);
            
            return isClose && countEntries > 0;
        }
    }
}