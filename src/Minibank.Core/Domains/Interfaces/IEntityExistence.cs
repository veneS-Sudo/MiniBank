using System.Threading;
using System.Threading.Tasks;

namespace Minibank.Core.Domains.Interfaces
{
    public interface IEntityExistence<T>
    {
        Task<bool> IsExistAsync(T value, CancellationToken cancellationToken);
    }
}