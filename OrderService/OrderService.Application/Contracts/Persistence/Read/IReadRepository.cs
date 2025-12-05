using OrderService.Domain.Common;
using System.Linq.Expressions;

namespace OrderService.Application.Contracts.Persistence.Read
{
    public interface IReadRepository<T> where T : BaseDomainModel
    {
        Task<T?> FindAsync(
            int id, 
            CancellationToken cancellationToken = default);
        Task<T?> GetEntityAsync(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, T>>? select = null,
            IEnumerable<Expression<Func<T, object>>>? includeProperties = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetListAsync(
            Expression<Func<T, bool>>? predicate = null,
            Expression<Func<T, T>>? select = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            IEnumerable<Expression<Func<T, object>>>? includeProperties = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> FromSqlAsync(
            FormattableString sql,
            CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);
        Task<int> CountAsync(
            CancellationToken cancellationToken = default);
    }
}
