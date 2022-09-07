using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Repositories;
using Minibank.Core.Exceptions.FriendlyExceptions;
using Minibank.Data.DatabaseLayer.Context;

namespace Minibank.Data.DatabaseLayer.DbModels.Transfers.Repositories
{
    public class MoneyTransferRepository : IMoneyTransferRepository
    {
        private readonly MinibankContext _context;
        private readonly IMapper _mapper;

        public MoneyTransferRepository(MinibankContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<MoneyTransfer> GetByIdAsync(string id, CancellationToken cancellationToken)
        {
            var transferEntity = await _context.AmountTransfers
                .AsNoTracking()
                .FirstOrDefaultAsync(transfer => transfer.Id == id, cancellationToken);
            
            if (transferEntity == null)
            {
                throw new ObjectNotFoundException($"перевод с id: {id}, не найден");
            }

            return _mapper.Map<MoneyTransfer>(transferEntity);
        }

        public async Task<List<MoneyTransfer>> GetAllTransfersAsync(CancellationToken cancellationToken)
        {
            var transfers = await _context.AmountTransfers
                .AsNoTracking()
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<MoneyTransferEntity>, List<MoneyTransfer>>(transfers);
        }

        public async Task<List<MoneyTransfer>> GetAllTransfersAsync(string bankAccountId, CancellationToken cancellationToken)
        {
            var transfers = await _context.AmountTransfers
                .AsNoTracking()
                .Where(transfer => transfer.FromBankAccountId == bankAccountId || transfer.ToBankAccountId == bankAccountId)
                .ToListAsync(cancellationToken);
            return _mapper.Map<List<MoneyTransferEntity>, List<MoneyTransfer>>(transfers);
        }

        public async Task<string> CreateTransferAsync(decimal amount, string fromAccountId, string toAccountId, Currency currency, CancellationToken cancellationToken)
        {
            var entity = new MoneyTransferEntity
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                Currency = currency,
                FromBankAccountId = fromAccountId,
                ToBankAccountId = toAccountId
            };

            var createMoneyTransfer = await _context.AmountTransfers.AddAsync(entity, cancellationToken);
            
            return createMoneyTransfer.Entity.Id;
        }

        public async Task<string> CreateTransferAsync(MoneyTransfer moneyTransfer, CancellationToken cancellationToken)
        {
            var transferEntity = _mapper.Map<MoneyTransferEntity>(moneyTransfer);
            transferEntity.Id = Guid.NewGuid().ToString();

            var createMoneyTransfer = await _context.AmountTransfers.AddAsync(transferEntity, cancellationToken);
            
            return createMoneyTransfer.Entity.Id;
        }
    }
}