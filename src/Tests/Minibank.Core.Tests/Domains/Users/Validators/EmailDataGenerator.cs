using System.Collections;
using System.Collections.Generic;

namespace Minibank.Core.Tests.Domains.Users.Validators
{
    public class EmailDataGenerator : IEnumerable<object[]>
    {
        private readonly List<object[]> _data = new()
        {
            new object[] { "@mail.com" },
            new object[] { "@" },
            new object[] { "." },
            new object[] { "@.com" },
            new object[] { "mail.com" },
            new object[] { "mail@" },
            new object[] {" "},
            new object[] {"_"}
        };
        
        public IEnumerator<object[]> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}