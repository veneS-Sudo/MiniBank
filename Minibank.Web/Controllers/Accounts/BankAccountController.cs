using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Transfers;
using Minibank.Web.Controllers.Accounts.Dto;
using Minibank.Web.Controllers.MoneyTransfers.Dto;

namespace Minibank.Web.Controllers.Accounts
{
    [ApiController]
    [Route("api/[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _accountService;
        private readonly IMapper _mapper;

        public BankAccountController(IBankAccountService accountService, IMapper mapper)
        {
            _accountService = accountService;
            _mapper = mapper;
        }

        [HttpGet("GetAccount/{id}")]
        public async Task<GetBankAccountDto> GetAccountById(string id)
        {
            var account = await _accountService.GetByIdAsync(id);
            return _mapper.Map<GetBankAccountDto>(account);
        }

        [HttpGet("GetAccounts")]
        public async Task<List<GetBankAccountDto>> GetAllAccount()
        {
            return (await _accountService.GetAllAccountsAsync())
                .Select(account => _mapper.Map<GetBankAccountDto>(account)).ToList();
        }

        [HttpPost("/CreateTransfer")]
        public async Task TransferAmount(CreateMoneyTransferDto amountMoneyTransfer)
        {
            var fromAccount = await _accountService.GetByIdAsync(amountMoneyTransfer.FromBankAccountId);
            var targetTransfer = _mapper.Map<MoneyTransfer>(amountMoneyTransfer);
            targetTransfer.Currency = fromAccount.Currency;
            
            await _accountService.TransferAmountAsync(targetTransfer);
        }

        [HttpPost("CreateAccount")]
        public Task CreateAccount(CreateBankAccountDto account)
        {
            return _accountService.CreateAccountAsync(_mapper.Map<BankAccount>(account));
        }

        [HttpPut("UpdateAccount/{accountId}")]
        public Task UpdateAccount(string accountId, UpdateAccountDto account)
        {
            var targetAccount = _mapper.Map<BankAccount>(account);
            targetAccount.Id = accountId;
            
            return _accountService.UpdateAccountAsync(targetAccount);
        }

        [HttpDelete("DeleteAccount")]
        public Task CloseAccount(string accountId)
        {
            return _accountService.CloseAccountAsync(accountId);
        }
    }
}