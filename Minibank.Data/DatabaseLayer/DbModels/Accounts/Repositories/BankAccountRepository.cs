using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<BankAccount> GetByIdAsync(string id)
        {
            var bankAccountEntity = await _context.BankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.Id == id);
            if (bankAccountEntity == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {id}, не найден");
            }

            return _mapper.Map<BankAccount>(bankAccountEntity);
        }

        public Task<List<BankAccount>> GetAllAccountsAsync()
        {
            return _context.BankAccounts
                .AsNoTracking()
                .Select(account => _mapper.Map<BankAccount>(account))
                .ToListAsync();
        }

        public Task CreateAccountAsync(BankAccount bankAccount)
        {
            var bankAccountEntity = _mapper.Map<BankAccountEntity>(bankAccount);
            bankAccountEntity.Id = Guid.NewGuid().ToString();
            bankAccountEntity.IsOpen = true;
            bankAccountEntity.DateOpen = DateTime.UtcNow;
            
            return _context.BankAccounts.AddAsync(bankAccountEntity).AsTask();
        }

        public async Task UpdateAccountAsync(BankAccount bankAccount)
        {
            var targetAccount = await _context.BankAccounts.FindAsync(bankAccount.Id);

            if (targetAccount == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {bankAccount.Id}, не найден");
            }
            
            targetAccount.Balance = bankAccount.Balance;
        }

        public Task<bool> ExistsByUserIdAsync(string userId)
        {
            return _context.BankAccounts
                .AsNoTracking()
                .AnyAsync(account => account.UserId == userId);
        }

        public async Task CloseAccountAsync(string id)
        {
            var bankAccountEntity = await _context.BankAccounts.FindAsync(id);
            if (bankAccountEntity == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {id}, не найден");
            }
            
            bankAccountEntity.IsOpen = false;
            bankAccountEntity.DateClose = DateTime.UtcNow;
        }

        public async Task<bool> BankAccountIsOpenAsync(string id)
        {
            var bankAccountEntity = await _context.BankAccounts
                .AsNoTracking()
                .FirstOrDefaultAsync(account => account.Id == id);
            if (bankAccountEntity == null)
            {
                throw new ObjectNotFoundException($"аккаунт с id: {id}, не найден");
            }

            return bankAccountEntity.IsOpen;
        }
    }
}