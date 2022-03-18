using Minibank.Core.Converters;
using Minibank.Core.Domains.Accounts;

namespace Minibank.Core.Domains.Users
{
    public class Transfer
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public Currency Currency { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
    }
}