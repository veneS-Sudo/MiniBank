using Minibank.Core.Converters;

namespace Minibank.Web.Controllers.Accounts.Dto
{
    public class CreateBankAccountDto
    {
        public string UserId { get; set; }
        public double Balance { get; set; } 
        public Currency Currency { get; set; }
    }
}