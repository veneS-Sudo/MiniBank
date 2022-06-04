using System.Collections.Generic;
using Minibank.Core.Domains.Transfers;

namespace Minibank.Core.Tests.Domains.Transfers.EqualityComparers
{
    public class MoneyTransferWithoutIdEqualityComparer : EqualityComparer<MoneyTransfer>
    {
        public override bool Equals(MoneyTransfer? x, MoneyTransfer? y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            return (x.FromBankAccountId == y.FromBankAccountId &&
                    x.ToBankAccountId == y.ToBankAccountId &&
                    x.Currency == y.Currency &&
                    x.Amount == y.Amount);
        }

        public override int GetHashCode(MoneyTransfer obj)
        {
            var hCode = obj.FromBankAccountId.GetHashCode() * 17 + obj.ToBankAccountId.GetHashCode() * 17 +
                        obj.Amount.GetHashCode() + (int)obj.Currency;
            return hCode;
        }
    }
}