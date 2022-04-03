using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<MoneyTransfer> GetByIdAsync(string id)
        {
            var transferEntity = await _context.AmountTransfers
                .AsNoTracking()
                .FirstOrDefaultAsync(transfer => transfer.Id == id);
            
            if (transferEntity == null)
            {
                throw new ObjectNotFoundException($"перевод с id: {id}, не найден");
            }

            return _mapper.Map<MoneyTransfer>(transferEntity);
        }

        public Task<List<MoneyTransfer>> GetAllTransfersAsync()
        {
            return _context.AmountTransfers
                .AsNoTracking()
                .Select(transferEntity => _mapper.Map<MoneyTransfer>(transferEntity))
                .ToListAsync();
        }

        public Task CreateTransferAsync(double amount, string fromAccountId, string toAccountId, Currency currency)
        {
            var entity = new MoneyTransferEntity
            {
                Id = Guid.NewGuid().ToString(),
                Amount = amount,
                Currency = currency,
                FromBankAccountId = fromAccountId,
                ToBankAccountId = toAccountId
            };
            
            return _context.AmountTransfers.AddAsync(entity).AsTask();
        }

        public Task CreateTransferAsync(MoneyTransfer moneyTransfer)
        {
            var transferEntity = _mapper.Map<MoneyTransferEntity>(moneyTransfer);
            transferEntity.Id = Guid.NewGuid().ToString();
            
            return _context.AmountTransfers.AddAsync(transferEntity).AsTask();
        }
    }
}