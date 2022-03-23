using System.Collections.Generic;
using Minibank.Core.Converters;

namespace Minibank.Core.Domains.Transfers.Repositories
{
    public interface ITransferRepository
    {
        Transfer GetById(string id);
        List<Transfer> GetAllTransfers();
        void CreateTransfer(double amount, string fromAccountId, string toAccountId, Currency currency);
        void CreateTransfer(Transfer transfer);
        void UpdateTransfer(Transfer transfer);
        void DeleteTransfer(string id);
    }
}