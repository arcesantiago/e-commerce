using Microsoft.EntityFrameworkCore;

namespace OrderService.Infrastructure.Repositories
{
    public abstract class UnitOfWorkBase<TContext> : IDisposable where TContext : DbContext
    {
        protected readonly TContext _context;
        protected UnitOfWorkBase(TContext context) => _context = context;
        public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => await _context.SaveChangesAsync(cancellationToken);
        public void Dispose() => _context.Dispose();
    }
}