using System.Collections.Generic;
using Minibank.Core.Converters;

namespace Minibank.Core.Domains.Transfers.Repositories
{
    public interface ITransferRepository
    {
        public Transfer GetById(string id);
        public List<Transfer> GetAllTransfers();
        public void CreateTransfer(double amount, string fromAccountId, string toAccountId, Currency currency);
        public void CreateTransfer(Transfer transfer);
        public void UpdateTransfer(Transfer transfer);
        public void DeleteTransfer(string id);
    }
}