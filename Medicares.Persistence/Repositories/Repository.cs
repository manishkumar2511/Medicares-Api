using System.Linq.Expressions;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Medicares.Domain.Base;

namespace Medicares.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _dbContext;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<T>();
    }

    public IQueryable<T> Entities => _dbSet;

    public IQueryable<T> FromSqlRaw(string sql, params object[] parameters)
    {
        return _dbSet.FromSqlRaw(sql, parameters);
    }

    public virtual async Task<T?> GetByIdAsync(object id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
        return entity;
    }

    public virtual async Task<List<T>> AddRangeAsync(List<T> entities)
    {
        await _dbSet.AddRangeAsync(entities);
        return entities;
    }
    
    public virtual async Task<List<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize)
    {
        return await _dbSet.Skip((pageNumber - 1) * pageSize)
                           .Take(pageSize)
                           .AsNoTracking()
                           .ToListAsync();
    }

    public virtual Task UpdateAsync(T entity)
    {
         _dbContext.Entry(entity).State = EntityState.Modified;
        // _dbSet.Update(entity); // Alternative
        return Task.CompletedTask;
    }

    public virtual Task UpdateColumnAsync(T entity, string[] ColumnNames)
    {
        throw new NotImplementedException("Column specific update not implemented yet, use full update or custom logic.");
    }
    
    public virtual Task UpdateRangeAsync(IList<T> entities)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteRangeAsync(IList<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> EntityExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }

    public virtual async Task<List<T>> GetWithSpecificationAsync(Medicares.Application.Contracts.Specifications.ISpecification<T> spec, CancellationToken ct = default)
    {
        IQueryable<T> query = _dbSet;

        if (spec.Criteria != null)
        {
            query = query.Where(spec.Criteria);
        }

        query = spec.Includes.Aggregate(query, (current, include) => current.Include(include));
        query = spec.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));

        if (!string.IsNullOrEmpty(spec.OrderBy))
        {
            // Simple order by for now, can be expanded to dynamic ordering helper
            // For a production app, use a more robust dynamic LINQ or similar
            // This is a minimal implementation to show the pattern
            // query = query.OrderBy(spec.OrderBy); 
        }

        return await query.AsNoTracking().ToListAsync(ct);
    }
}
