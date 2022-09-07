using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Transfers.Services;
using Minibank.Web.Controllers.Accounts.Dto;
using Minibank.Web.Controllers.MoneyTransfers.Dto;

namespace Minibank.Web.Controllers.Accounts
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _accountService;
        private readonly IMoneyTransferService _moneyTransferService;
        private readonly IMapper _mapper;

        public BankAccountController(IBankAccountService accountService, IMapper mapper, IMoneyTransferService moneyTransferService)
        {
            _accountService = accountService;
            _mapper = mapper;
            _moneyTransferService = moneyTransferService;
        }

        [HttpGet("[action]")]
        public async Task<GetBankAccountDto> GetAccountById(string accountId, CancellationToken cancellationToken)
        {
            var account = await _accountService.GetByIdAsync(accountId, cancellationToken);
            return _mapper.Map<GetBankAccountDto>(account);
        }

        [HttpGet("[action]")]
        public async Task<List<GetBankAccountDto>> GetAllAccount(CancellationToken cancellationToken)
        {
            var bankAccounts = await _accountService.GetAllAccountsAsync(cancellationToken);
            return _mapper.Map<List<BankAccount>, List<GetBankAccountDto>>(bankAccounts);
        }

        [HttpPost("[action]")]
        public async Task<string> TransferAmount(CreateMoneyTransferDto amountMoneyTransfer, CancellationToken cancellationToken)
        {
            var fromAccount = await _accountService.GetByIdAsync(amountMoneyTransfer.FromBankAccountId, cancellationToken);
            var targetTransfer = _mapper.Map<MoneyTransfer>(amountMoneyTransfer);
            targetTransfer.Currency = fromAccount.Currency;
            var createdMoneyTransferId = await _moneyTransferService.TransferAmountAsync(targetTransfer, cancellationToken);
            
            return createdMoneyTransferId;
        }

        [HttpPost("[action]")]
        public async Task<string> CreateAccount(CreateBankAccountDto account, CancellationToken cancellationToken)
        {
            var targetBankAccount = _mapper.Map<BankAccount>(account);
            var createdBankAccount = await _accountService.CreateAccountAsync(targetBankAccount, cancellationToken);
            return createdBankAccount.Id;
        }

        [HttpPut("[action]/{accountId}")]
        public async Task<bool> UpdateAccount(string accountId, UpdateAccountDto account, CancellationToken cancellationToken)
        {
            var targetAccount = _mapper.Map<BankAccount>(account);
            targetAccount.Id = accountId;
            
            var bankAccountUpdated = await _accountService.UpdateAccountAsync(targetAccount, cancellationToken);
            return bankAccountUpdated;
        }

        [HttpDelete("[action]/{accountId}")]
        public async Task<bool> CloseAccount(string accountId, CancellationToken cancellationToken)
        {
            return await _accountService.CloseAccountAsync(accountId, cancellationToken);
        }
    }
}