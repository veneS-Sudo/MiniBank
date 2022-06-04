using System;
using System.Threading;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Interfaces;
using Moq;

namespace Minibank.Core.Tests.Mocks.MockExtensions
{
    public static class MockVerifyExtensions
    {
        public static void VerifyExist<T, TValue>(this Mock<T> mock, TValue value, Func<Times> times)
            where T : class, IEntityExistence<TValue>
        {
            mock.Verify(_ => _.IsExistAsync(value, It.IsAny<CancellationToken>()), times);
        }

        public static void VerifyOpenness(this Mock<IBankAccountRepository> mock, string value, Func<Times> times)
        {
            mock.Verify(_ => _.IsOpenAsync(value, It.IsAny<CancellationToken>()), times);
        }
    }
}