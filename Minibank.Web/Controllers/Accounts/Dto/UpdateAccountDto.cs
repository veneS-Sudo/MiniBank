using Minibank.Core.Converters;

namespace Minibank.Web.Controllers.Accounts.Dto
{
    public class UpdateAccountDto
    {
        public double Balance { get; set; }
        public Currency Currency { get; set; }
    }
}