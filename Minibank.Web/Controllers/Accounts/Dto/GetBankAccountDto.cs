using System;
using Minibank.Core.Converters;

namespace Minibank.Web.Controllers.Accounts.Dto
{
    public class GetBankAccountDto
    {
        public decimal Balance { get; set; }
        public Currency Currency { get; set; }
        public bool IsOpen { get; set; }
    }
}