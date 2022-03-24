using Minibank.Core.Converters;

namespace Minibank.Web.Controllers.Accounts.Dto
{
    public class CreateAccountDto
    {
        public string UserId { get; set; }
        public Currency Currency { get; set; }
    }
}