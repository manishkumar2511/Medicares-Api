using System.Linq.Dynamic.Core;
using Medicares.Application.Contracts.Specifications;
using Medicares.Application.Contracts.Wrappers;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Application.Extensions;

public static class QueryableExtensions
{
    public static async Task<PaginatedResult<T>> ToPaginatedListAsync<T>(this IQueryable<T> source, int pageNumber, int pageSize, CancellationToken ct = default) where T : class
    {
        ArgumentNullException.ThrowIfNull(source);
        pageNumber = pageNumber == 0 ? 1 : pageNumber;
        pageSize = pageSize == 0 ? int.MaxValue : pageSize;
        int count = await source.CountAsync(ct);
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        List<T> items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
    }

    public static PaginatedResult<T> ToPaginatedList<T>(this List<T> source, int pageNumber, int pageSize) where T : class
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        pageNumber = pageNumber == 0 ? 1 : pageNumber;
        pageSize = pageSize == 0 ? int.MaxValue : pageSize;
        int count = source.Count;
        pageNumber = pageNumber <= 0 ? 1 : pageNumber;
        List<T> items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        return PaginatedResult<T>.Success(items, count, pageNumber, pageSize);
    }

    public static IQueryable<T> Specify<T>(this IQueryable<T> query, ISpecification<T> spec) where T : class
    {
        IQueryable<T> queryable = query;

        //Apply expression-based includes
        if (spec.Includes.Any())
        {
            queryable = spec.Includes.Aggregate(queryable, (current, include) => current.Include(include));
        }

        // Apply string-based includes
        if (spec.IncludeStrings.Any())
        {
            queryable = spec.IncludeStrings.Aggregate(queryable, (current, include) => current.Include(include));
        }

        // Apply sorting (requires System.Linq.Dynamic.Core if using string)
        if (!string.IsNullOrEmpty(spec.OrderBy))
        {
            queryable = queryable.OrderBy(spec.OrderBy);
        }

        // Apply filter criteria
        if (spec.Criteria != null)
        {
            queryable = queryable.Where(spec.Criteria);
        }

        return queryable;
    }
}
