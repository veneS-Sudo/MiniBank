using System.Collections.Generic;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Domains.Accounts.Services
{
    public interface IBankAccountService
    {
        BankAccount GetById(string id);
        List<BankAccount> GetAllAccounts();
        void CreateAccount(string id, Currency currency);
        void UpdateAccount(BankAccount account);
        void CloseAccount(string id);
        double CalculateCommission(Transfer transfer);
        void TransferAmount(Transfer transfer);
    }
}