using Microsoft.EntityFrameworkCore;
using ProductService.Application.Common.Validation;
using ProductService.Application.Contracts.Persistence.Read;
using ProductService.Application.Contracts.Persistence.Write;
using ProductService.Application.Models;
using ProductService.Domain.Common;
using ProductService.Infrastructure.Percistence;
using System.Linq.Expressions;

namespace ProductService.Infrastructure.Repositories
{
    public class RepositoryBase<T> : IReadRepository<T>, IWriteRepository<T> where T : BaseDomainModel
    {
        protected readonly ProductDbContext _context;
        protected readonly DbSet<T> _dbSet;
        public RepositoryBase(ProductDbContext context, DbSet<T> dbSet)
        {
            _context = context;
            _dbSet = dbSet;
        }
        public async Task<T?> FindAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public async Task<T?> GetEntityAsync(Expression<Func<T, bool>> predicate, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (includes is not null) query = includes.Aggregate(query, (current, include) => current.Include(include));

            return await query.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (includes is not null) query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();

            if (predicate is not null) query = query.Where(predicate);

            if (orderBy is not null) query = orderBy(query);

            return await query.ToListAsync(cancellationToken);
        }

        public async Task<PagedResult<T>> GetListPaginatedAsync(int currentPage, int pageSize, Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, List<Expression<Func<T, object>>>? includes = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<T> query = _dbSet;

            if (disableTracking) query = query.AsNoTracking();

            if (includes is not null) query = includes.Aggregate(query, (current, include) => current.Include(include)).AsSplitQuery();

            if (predicate is not null) query = query.Where(predicate);

            if (orderBy is not null) query = orderBy(query);

            var skip = (currentPage - 1) * pageSize;

            var rowsCount = await query.CountAsync(cancellationToken);

            var results = await query
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<T>(results, rowsCount, currentPage, pageSize);
        }

        public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            return entity;
        }

        public T Update(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public T UpdateFields(T entity, Expression<Func<T, object>>[] propertiesToUpdate, CancellationToken cancellationToken = default)
        {
            UpdateFieldValidator.Validate(propertiesToUpdate);

            _context.Attach(entity);

            foreach (var property in propertiesToUpdate)
            {
                _context.Entry(entity).Property(property).IsModified = true;
            }

            return entity;
        }

        public async Task<int> UpdateManyAsync(Func<IQueryable<T>, IQueryable<T>> filter, Func<IQueryable<T>, Task<int>> updateAction, CancellationToken cancellationToken = default)
        {
            var query = filter(_dbSet);
            return await updateAction(query);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task<int> DeleteManyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet
            .Where(predicate)
            .ExecuteDeleteAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<T>> FromSqlAsync(FormattableString sql, CancellationToken cancellationToken = default)
        {
            return await _dbSet
            .FromSqlInterpolated(sql)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await _dbSet.AnyAsync(predicate, cancellationToken);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return _dbSet.CountAsync(cancellationToken);
        }
    }
}