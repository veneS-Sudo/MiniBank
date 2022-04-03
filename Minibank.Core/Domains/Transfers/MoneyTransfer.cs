using Minibank.Core.Converters;

namespace Minibank.Core.Domains.Transfers
{
    public class MoneyTransfer
    {
        public string Id { get; set; }
        public double Amount { get; set; }
        public Currency Currency { get; set; }
        public string FromBankAccountId { get; set; }
        public string ToBankAccountId { get; set; }
    }
}