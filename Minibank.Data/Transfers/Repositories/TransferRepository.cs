using System;
using System.Collections.Generic;
using System.Linq;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Repositories;
using Minibank.Core.Exceptions.FriendlyExceptions;

namespace Minibank.Data.Transfers.Repositories
{
    public class TransferRepository : ITransferRepository
    {
        private static List<TransferEntity> _transferHistory = new();
        
        public Transfer GetById(string id)
        {
            var entity = _transferHistory.FirstOrDefault(transfer => transfer.Id == id);
            if (entity == null)
            {
                throw new ObjectNotFoundException($"Перевод с id:{id}, не найден!");
            }

            return new Transfer
            {
                Id = entity.Id,
                Amount = entity.Amount,
                Currency = entity.Currency,
                FromAccountId = entity.FromAccountId,
                ToAccountId = entity.ToAccountId
            };
        }

        public List<Transfer> GetAllTransfers()
        {
            return _transferHistory.Select(transfer => new Transfer
            {
                Id = transfer.Id,
                Amount = transfer.Amount,
                Currency = transfer.Currency,
                FromAccountId = transfer.FromAccountId,
                ToAccountId = transfer.ToAccountId
            }).ToList();
        }

        public void CreateTransfer(double amount, string fromAccountId, string toAccountId, Currency currency)
        {
            var entity = new TransferEntity
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                Currency = currency,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            };
            
            _transferHistory.Add(entity);
        }

        public void CreateTransfer(Transfer transfer)
        {
            var entity = new TransferEntity
            {
                Id = Guid.NewGuid().ToString(),
                Amount = transfer.Amount,
                Currency = transfer.Currency,
                FromAccountId = transfer.FromAccountId,
                ToAccountId = transfer.ToAccountId
            };
            
            _transferHistory.Add(entity);
        }

        public void UpdateTransfer(Transfer transfer)
        {
            var targetTransfer = _transferHistory.FirstOrDefault(entity => entity.Id == transfer.Id);
            if (targetTransfer == null)
            {
                throw new ObjectNotFoundException($"Перевод с id:{transfer.Id}, не найден!");
            }

            targetTransfer.Amount = transfer.Amount;
            targetTransfer.Currency = transfer.Currency;
            targetTransfer.FromAccountId = transfer.FromAccountId;
            targetTransfer.ToAccountId = transfer.ToAccountId;
        }

        public void DeleteTransfer(string id)
        {
            var entity = _transferHistory.FirstOrDefault(transfer => transfer.Id == id);
            if (entity == null)
            {
                throw new ObjectNotFoundException($"Перевод с id:{id}, не найден!");
            }

            _transferHistory.Remove(entity);
        }
    }
}