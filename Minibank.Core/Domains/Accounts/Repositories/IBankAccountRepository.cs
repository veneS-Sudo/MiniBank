using System.Collections.Generic;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Accounts.Repositories
{
    public interface IBankAccountRepository
    {
        public BankAccount GetById(string id);
        public List<BankAccount> GetAllAccounts();
        public void CreateAccount(string id, Currency currency);
        public void UpdateAccount(BankAccount account);
        public bool ExistsByUserId(string id);
        public void CloseAccount(string id);
    }
}