using System.Collections.Generic;
using Minibank.Core.Converters;

namespace Minibank.Core.Domains.Accounts.Repositories
{
    public interface IBankAccountRepository
    {
        BankAccount GetById(string id);
        List<BankAccount> GetAllAccounts();
        void CreateAccount(string id, Currency currency);
        void UpdateAccount(BankAccount account);
        bool ExistsByUserId(string id);
        void CloseAccount(string id);
    }
}