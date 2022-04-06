using System.Threading;
using System.Threading.Tasks;
using Minibank.Core.Domains.Dal;

namespace Minibank.Data.DatabaseLayer.Context
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly MinibankContext _context;

        public UnitOfWork(MinibankContext context)
        {
            _context = context;
        }

        public int SaveChanges()
        {
            return _context.SaveChanges();
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }
    }
}