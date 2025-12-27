using Medicares.Application.Contracts.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Medicares.Application.Contracts.Extensions;

public static class UnitOfWorkExtensions
{
    public static async Task ExecuteTransactionAsync(this IUnitOfWork unitOfWork, Func<Task> action, CancellationToken ct = default)
    {
        using IDbContextTransaction transaction = await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            await action();
            await transaction.CommitAsync(ct);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }

    public static async Task<T> ExecuteTransactionAsync<T>(this IUnitOfWork unitOfWork, Func<Task<T>> action, CancellationToken ct = default)
    {
        using IDbContextTransaction transaction = await unitOfWork.BeginTransactionAsync(ct);
        try
        {
            T result = await action();
            await transaction.CommitAsync(ct);
            return result;
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}
