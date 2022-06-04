using System.Collections.Generic;
using Minibank.Core.Domains.Accounts;

namespace Minibank.Core.Tests.Domains.Accounts.EqualityComparers
{
    public class BankAccountWithoutIdEqualityComparer : EqualityComparer<BankAccount>
    {
        public override bool Equals(BankAccount? x, BankAccount? y)
        {
            if (x == null && y == null)
                return true;
            else if (x == null || y == null)
                return false;
            return (x.UserId == y.UserId && x.Balance == y.Balance &&
                    x.Currency == y.Currency && x.IsOpen == y.IsOpen &&
                   x.DateOpen == y.DateOpen && x.DateClose == y.DateClose);
        }

        public override int GetHashCode(BankAccount obj)
        {
            var hCode = obj.UserId.GetHashCode() * 17 + (int)obj.Currency + obj.IsOpen.GetHashCode() +
                        obj.DateOpen.GetHashCode() + obj.DateClose.GetHashCode();
            return hCode;
        }
    }
}