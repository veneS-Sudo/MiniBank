using System.Threading.Tasks;

namespace Minibank.Core.Domains.Dal
{
    public interface IUnitOfWork
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}