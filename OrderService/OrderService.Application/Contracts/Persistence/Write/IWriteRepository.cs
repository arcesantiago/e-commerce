using OrderService.Domain.Common;
using System.Linq.Expressions;

namespace OrderService.Application.Contracts.Persistence.Write
{
    public interface IWriteRepository<T> where T : BaseDomainModel
    {
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

        Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

        Task<T> UpdateFieldsAsync(
            T entity,
            Expression<Func<T, object>>[] propertiesToUpdate,
            CancellationToken cancellationToken = default);

        public Task<int> UpdateManyAsync(
            Func<IQueryable<T>, IQueryable<T>> filter,
            Func<IQueryable<T>, Task<int>> updateAction,
            CancellationToken cancellationToken = default);
        Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
        Task<int> DeleteManyAsync(
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default);
    }

}
