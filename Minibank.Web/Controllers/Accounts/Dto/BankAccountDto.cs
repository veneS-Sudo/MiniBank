using System;
using Minibank.Core.Converters;

namespace Minibank.Web.Controllers.Accounts.Dto
{
    public class BankAccountDto
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public double Balance { get; set; }
        public Currency Currency { get; set; }
        public bool IsOpen { get; set; }
        public DateTime DateOpen { get; set; }
        public DateTime DateClose { get; set; }
    }
}