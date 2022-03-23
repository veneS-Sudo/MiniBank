using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Transfers;
using Minibank.Web.Controllers.Accounts.Dto;
using Minibank.Web.Controllers.MoneyTransfers.Dto;

namespace Minibank.Web.Controllers.Accounts
{
    [ApiController]
    [Route("[controller]")]
    public class BankAccountController : ControllerBase
    {
        private readonly IBankAccountService _accountService;

        public BankAccountController(IBankAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet("{id}")]
        public BankAccountDto GetAccountById(string id)
        {
            var entity = _accountService.GetById(id);
            return new BankAccountDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Balance = entity.Balance,
                Currency = entity.Currency,
                IsOpen = entity.IsOpen,
                DateOpen = entity.DateOpen,
                DateClose = entity.DateClose
            };
        }

        [HttpGet]
        public List<BankAccountDto> GetAllAccount()
        {
            return _accountService.GetAllAccounts().Select(account => new BankAccountDto
            {
                Id = account.Id,
                UserId = account.UserId,
                Balance = account.Balance,
                Currency = account.Currency,
                IsOpen = account.IsOpen,
                DateOpen = account.DateOpen,
                DateClose = account.DateClose
            }).ToList();
        }

        [HttpPost("/transfer")]
        public void TransferAmount(CreateTransferDto amountTransfer)
        {
            _accountService.TransferAmount(new Transfer
            {
                Amount = amountTransfer.Amount,
                Currency = _accountService.GetById(amountTransfer.FromAccountId).Currency,
                FromAccountId = amountTransfer.FromAccountId,
                ToAccountId = amountTransfer.ToAccountId
            });
        }

        [HttpPost]
        public void CreateAccount(CreateAccountDto account)
        {
            _accountService.CreateAccount(account.UserId, account.Currency);
        }

        [HttpPut("{accountId}")]
        public void UpdateAccount(string accountId, UpdateAccountDto account)
        {
            _accountService.UpdateAccount(new BankAccount
            {
                Id = accountId,
                Balance = account.Balance,
                Currency = account.Currency
            });
        }

        [HttpDelete("{accountId}")]
        public void CloseAccount(string accountId)
        {
            _accountService.CloseAccount(accountId);
        }
    }
}