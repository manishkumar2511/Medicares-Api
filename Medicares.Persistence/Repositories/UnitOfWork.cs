using System.Collections;
using Medicares.Application.Contracts.Interfaces.Repositories;
using Medicares.Persistence.Context;
using Medicares.Domain.Base;
using Microsoft.EntityFrameworkCore.Storage;

namespace Medicares.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private readonly Hashtable _repositories;

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _repositories = new Hashtable();
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        IDbContextTransaction transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        return transaction;
    }

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        string type = typeof(T).Name;

        if (!_repositories.ContainsKey(type))
        {
            Type repositoryType = typeof(Repository<>);
            object repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(T)), _dbContext)!;
            _repositories.Add(type, repositoryInstance);
        }

        IRepository<T> repository = (IRepository<T>)_repositories[type]!;
        return repository;
    }

    public void SetOwnerId(Guid ownerId)
    {
        _dbContext.CurrentOwnerId = ownerId;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        int result = await _dbContext.SaveChangesAsync(cancellationToken);
        return result;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }
}
