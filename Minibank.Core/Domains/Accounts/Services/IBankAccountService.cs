using System.Collections.Generic;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Accounts.Services
{
    public interface IBankAccountService
    {
        public BankAccount GetById(string id);
        public IEnumerable<BankAccount> GetAllAccounts();
        public void CreateAccount(string id, Currency currency);
        public void UpdateAccount(BankAccount account);
        public void CloseAccount(string id);
        public double CalculateCommission(Transfer transfer);
        public void TransferAmount(Transfer transfer);
    }
}