using System;
using System.Collections.Generic;
using System.Linq;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Exceptions.FriendlyException;

namespace Minibank.Data.Users.Accounts.Repositories
{
    public class BankAccountRepository : IBankAccountRepository
    {
        private static List<BankAccountEntity> _accountEntities = new();

        public BankAccount GetById(string id)
        {
            var entity = _accountEntities.FirstOrDefault(account => account.Id.Equals(id));
            if (entity == null)
            {
                throw new ObjectNotFoundException($"Ааккаунт с id:{id}, не найден!");
            }

            return new BankAccount
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Balance = entity.Balance,
                Currency = entity.Currency,
                IsOpen = entity.IsOpen,
                DateOpen = entity.DateOpen,
                DateClose = entity.DateClose
            };
        }

        public List<BankAccount> GetAllAccounts()
        {
            return _accountEntities.Select(accountModel => new BankAccount
            {
                Id = accountModel.Id,
                UserId = accountModel.UserId,
                Balance = accountModel.Balance,
                Currency = accountModel.Currency,
                IsOpen = accountModel.IsOpen,
                DateOpen = accountModel.DateOpen,
                DateClose = accountModel.DateClose
            }).ToList();
        }

        public void CreateAccount(string id, Currency currency)
        {
            var entity = new BankAccountEntity
            {
                Id = Guid.NewGuid().ToString(),
                UserId = id,
                Balance = 0,
                Currency = currency,
                IsOpen = true,
                DateOpen = DateTime.UtcNow
            };
            _accountEntities.Add(entity);
        }

        public void UpdateAccount(BankAccount account)
        {
            var targetAccount = _accountEntities.FirstOrDefault(entity => entity.Id == account.Id);

            if (targetAccount == null)
            {
                throw new ObjectNotFoundException($"Ааккаунт с id:{account.Id}, не найден!");
            }
            
            targetAccount.Balance = account.Balance;
            targetAccount.Currency = account.Currency;
        }

        public bool ExistsByUserId(string id)
        {
            return _accountEntities.Any(account => account.UserId == id);
        }

        public void CloseAccount(string id)
        {
            var entity = _accountEntities.FirstOrDefault(account => account.Id.Equals(id));
            if (entity == null)
            {
                throw new ObjectNotFoundException($"Ааккаунт с id:{id}, не найден!");
            }
            
            entity.IsOpen = false;
        }
    }
}