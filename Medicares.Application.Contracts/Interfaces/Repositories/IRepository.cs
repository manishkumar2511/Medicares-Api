using System.Linq.Expressions;
using Medicares.Domain.Base;

namespace Medicares.Application.Contracts.Interfaces.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> Entities { get; }
    IQueryable<T> FromSqlRaw(string sql, params object[] parameters);
    Task<T?> GetByIdAsync(object id);

    Task<List<T>> GetAllAsync();

    Task<List<T>> GetPagedResponseAsync(int pageNumber, int pageSize);

    Task<T> AddAsync(T entity, CancellationToken ct = default);
    Task<List<T>> AddRangeAsync(List<T> entities);

    Task UpdateAsync(T entity);
    Task UpdateColumnAsync(T entity, string[] ColumnNames);
    Task UpdateRangeAsync(IList<T> entities);

    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IList<T> entities);
    Task<bool> EntityExistsAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
}
