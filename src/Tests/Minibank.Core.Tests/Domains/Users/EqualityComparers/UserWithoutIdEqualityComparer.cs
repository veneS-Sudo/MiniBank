#nullable enable
using System.Collections.Generic;
using Minibank.Core.Domains.Users;

namespace Minibank.Core.Tests.Domains.Users.EqualityComparers
{
    public class UserWithoutIdEqualityComparer : EqualityComparer<User>
    {
        public override bool Equals(User? x, User? y)
        {
            if (x == null && y == null)
                return true;
            if (x == null || y == null)
                return false;
            return x.Login == y.Login && x.Email == y.Email;
        }

        public override int GetHashCode(User obj)
        {
            return obj.Login.GetHashCode() * 17 + obj.Email.GetHashCode() * 17;
        }
    }
}