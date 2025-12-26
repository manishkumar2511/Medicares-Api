using Medicares.Domain.Base;
using Microsoft.EntityFrameworkCore.Storage;

namespace Medicares.Application.Contracts.Interfaces.Repositories;
public interface IUnitOfWork : IDisposable
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    IRepository<T> Repository<T>() where T : BaseEntity;
    Task<int> SaveAsync(CancellationToken cancellationToken = default);
    void SetOwnerId(Guid ownerId);
}
