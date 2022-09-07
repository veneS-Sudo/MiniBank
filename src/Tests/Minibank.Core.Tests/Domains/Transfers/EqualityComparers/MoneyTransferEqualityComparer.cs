using System.Collections.Generic;
using Minibank.Core.Domains.Transfers;

namespace Minibank.Core.Tests.Domains.Transfers.EqualityComparers
{
    public class MoneyTransferEqualityComparer : EqualityComparer<MoneyTransfer>
    {
        public override bool Equals(MoneyTransfer x, MoneyTransfer y)
        {
            if (x is null && y is null)
                return true;
            if (x is null || y is null)
                return false;
            return (x.Id == y.Id && x.Amount == y.Amount && x.Currency == y.Currency &&
                    x.FromBankAccountId == y.FromBankAccountId && x.ToBankAccountId == y.ToBankAccountId);
        }

        public override int GetHashCode(MoneyTransfer obj)
        {
            var hCode = obj.Id.GetHashCode();
            return hCode;
        }
    }
}