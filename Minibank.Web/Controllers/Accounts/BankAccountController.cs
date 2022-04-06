﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public async Task<GetBankAccountDto> GetAccountById(string id, CancellationToken cancellationToken)
        {
            var account = await _accountService.GetByIdAsync(id, cancellationToken);
            return _mapper.Map<GetBankAccountDto>(account);
        }

        [HttpGet("GetAccounts")]
        public async Task<List<GetBankAccountDto>> GetAllAccount(CancellationToken cancellationToken)
        {
            var bankAccounts = await _accountService.GetAllAccountsAsync(cancellationToken); 
            return bankAccounts.Select(account => _mapper.Map<GetBankAccountDto>(account)).ToList();
        }

        [HttpPost("/CreateTransfer")]
        public async Task TransferAmount(CreateMoneyTransferDto amountMoneyTransfer, CancellationToken cancellationToken)
        {
            var fromAccount = await _accountService.GetByIdAsync(amountMoneyTransfer.FromBankAccountId, cancellationToken);
            var targetTransfer = _mapper.Map<MoneyTransfer>(amountMoneyTransfer);
            targetTransfer.Currency = fromAccount.Currency;
            
            await _accountService.TransferAmountAsync(targetTransfer, cancellationToken);
        }

        [HttpPost("CreateAccount")]
        public Task CreateAccount(CreateBankAccountDto account, CancellationToken cancellationToken)
        {
            var targetBankAccount = _mapper.Map<BankAccount>(account);
            return _accountService.CreateAccountAsync(targetBankAccount, cancellationToken);
        }

        [HttpPut("UpdateAccount/{accountId}")]
        public Task UpdateAccount(string accountId, UpdateAccountDto account, CancellationToken cancellationToken)
        {
            var targetAccount = _mapper.Map<BankAccount>(account);
            targetAccount.Id = accountId;
            
            return _accountService.UpdateAccountAsync(targetAccount, cancellationToken);
        }

        [HttpDelete("DeleteAccount")]
        public Task CloseAccount(string accountId, CancellationToken cancellationToken)
        {
            return _accountService.CloseAccountAsync(accountId, cancellationToken);
        }
    }
}