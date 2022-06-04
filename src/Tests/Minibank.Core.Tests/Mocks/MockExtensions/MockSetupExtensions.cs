using System.Threading;
using Minibank.Core.Domains.Accounts.Repositories;
using Minibank.Core.Domains.Interfaces;
using Moq;
using Moq.Language.Flow;

namespace Minibank.Core.Tests.Mocks.MockExtensions
{
    public static class MockSetupExtensions
    {
        public static ISetup<T, bool> SetupExist<T, TValue>(this Mock<T> mock,
            TValue value) where T : class, IEntityExistence<TValue>
        {
            return mock.Setup(_ => _.IsExistAsync(value, It.IsAny<CancellationToken>()).Result);
        }

        public static ISetup<T, bool> SetupExist<T, TValue>(this Mock<T> mock, params TValue[] values)
            where T : class, IEntityExistence<TValue>
        {
            return mock.Setup(_ => _.IsExistAsync(It.IsIn(values), It.IsAny<CancellationToken>()).Result);
        }

        public static ISetup<IBankAccountRepository, bool> SetupOpenness(this Mock<IBankAccountRepository> mock,
            string value)
        {
            return mock.Setup(_ => _.IsOpenAsync(value, It.IsAny<CancellationToken>()).Result);
        }
        public static ISetup<IBankAccountRepository, bool> SetupOpenness(this Mock<IBankAccountRepository> mock,
            params string[] values)
        {
            return mock.Setup(_ => _.IsOpenAsync(It.IsIn(values), It.IsAny<CancellationToken>()).Result);
        }
    }
}