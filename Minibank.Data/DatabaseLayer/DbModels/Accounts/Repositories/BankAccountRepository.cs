using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Data.DatabaseLayer.Context;

namespace Minibank.Data.DatabaseLayer.DbModels.Accounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private readonly MinibankContext _context;
        private readonly IMapper _mapper;

        public BankAccountRepository(MinibankContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<BankAccount> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var bankAccountEntity = await _context.BankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.Id == id, cancellationToken);
            if (bankAccountEntity == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {id}, не найден");
            }

            return _mapper.Map<BankAccount>(bankAccountEntity);
        }

        public async Task<List<BankAccount>> GetAllAccountsAsync(CancellationToken cancellationToken)
        {
            return await _context.BankAccounts
                .AsNoTracking()
                .Select(account => _mapper.Map<BankAccount>(account))
                .ToListAsync(cancellationToken);
        }

        public async Task<BankAccount> CreateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            var bankAccountEntity = _mapper.Map<BankAccountEntity>(bankAccount);
            bankAccountEntity.Id = Guid.NewGuid().ToString();
            bankAccountEntity.IsOpen = true;
            bankAccountEntity.DateOpen = DateTime.UtcNow;
            
            var createBankAccount = await _context.BankAccounts.AddAsync(bankAccountEntity, cancellationToken);
            
            return _mapper.Map<BankAccount>(createBankAccount.Entity);
        }

        public async Task<BankAccount> UpdateAccountAsync(BankAccount bankAccount, CancellationToken cancellationToken)
        {
            var targetAccount = await _context.BankAccounts.FindAsync( new object[] { bankAccount.Id }, cancellationToken);

            if (targetAccount == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {bankAccount.Id}, не найден");
            }
            
            targetAccount.Balance = bankAccount.Balance;

            return _mapper.Map<BankAccount>(targetAccount);
        }

        public async Task<bool> ExistsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return await _context.BankAccounts
                .AsNoTracking()
                .AnyAsync(account => account.UserId == userId, cancellationToken);
        }

        public async Task<bool> CloseAccountAsync(string id, CancellationToken cancellationToken)
        {
            var bankAccountEntity = await _context.BankAccounts.FindAsync(new object[] { id }, cancellationToken);
            if (bankAccountEntity == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {id}, не найден");
            }
            
            bankAccountEntity.IsOpen = false;
            bankAccountEntity.DateClose = DateTime.UtcNow;
            
            return !bankAccountEntity.IsOpen;
        }

        public async Task<bool> BankAccountIsOpenAsync(string id, CancellationToken cancellationToken)
        {
            var bankAccountEntity = await _context.BankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.Id == id, cancellationToken);
            if (bankAccountEntity == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {id}, не найден");
            }

            return bankAccountEntity.IsOpen;
        }
    }
}