using Minibank.Core.Converters;

namespace Minibank.Core.Domains.Transfers
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