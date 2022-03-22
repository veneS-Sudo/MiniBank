using Minibank.Core.Converters;

namespace Minibank.Data.Transfers
{
    public class TransferEntity
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public Currency Currency { get; set; }
        public string FromAccountId { get; set; }
        public string ToAccountId { get; set; }
    }
}