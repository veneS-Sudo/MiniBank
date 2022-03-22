using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;
using Minibank.Core.Domains.Accounts.Services;
using Minibank.Core.Domains.Transfers;
using Minibank.Core.Domains.Users;
using Minibank.Web.Controllers.Accounts.Dto;

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
        public IEnumerable<BankAccountDto> GetAllAccount()
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
            });
        }

        [HttpGet("/transfer")]
        public void TransferAmount(double amount, string fromAccountId, string toAccountId)
        {
            _accountService.TransferAmount(new Transfer
            {
                Amount = amount,
                Currency = _accountService.GetById(fromAccountId).Currency,
                FromAccountId = fromAccountId,
                ToAccountId = toAccountId
            });
        }

        [HttpPost("{userId}")]
        public void CreateAccount(string userId, Currency currency)
        {
            _accountService.CreateAccount(userId, currency);
        }

        [HttpPut("{accountId}")]
        public void UpdateAccount(string accountId, BankAccountDto account)
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