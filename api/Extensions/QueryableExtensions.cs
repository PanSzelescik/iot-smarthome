using System.Linq.Expressions;
using IotSmartHome.Data.Dto;
using Microsoft.EntityFrameworkCore;

namespace IotSmartHome.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, bool condition, Expression<Func<T, bool>> predicate)
        => condition ? source.Where(predicate) : source;
    
    public static IQueryable<T> SkipIf<T>(this IQueryable<T> source, bool condition, Func<int> countFunc)
        => condition ? source.Skip(countFunc()) : source;
    
    public static IQueryable<T> TakeIf<T>(this IQueryable<T> source, bool condition, Func<int> countFunc)
        => condition ? source.Take(countFunc()) : source;

    public static async Task<PaginatedResponse<TEntity>> ToPaginatedResponseAsync<TEntity>(
        this IQueryable<TEntity> queryable,
        int? skip,
        int? take,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await queryable.CountAsync(cancellationToken);
        var results = await queryable
            .SkipIf(skip.HasValue, () => skip!.Value)
            .TakeIf(take.HasValue, () => take!.Value)
            .ToListAsync(cancellationToken);
        
        return new PaginatedResponse<TEntity>
        {
            TotalCount = totalCount,
            Results = results,
        };
    }
}