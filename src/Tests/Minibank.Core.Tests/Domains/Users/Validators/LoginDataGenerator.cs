using System.Collections;
using System.Collections.Generic;

namespace Minibank.Core.Tests.Domains.Users.Validators
{
    public class LoginDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
            new object[] { " " },
            new object[] { "\n" },
            new object[] { "" },
            new object[] { null },
        };
        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}