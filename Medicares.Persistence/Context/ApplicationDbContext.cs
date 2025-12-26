using System.Linq.Expressions;
using System.Reflection;
using Medicares.Application.Contracts.Interfaces;
using Medicares.Domain.Base;
using Medicares.Domain.Entities.Auth;
using Medicares.Domain.Entities.Common;
using Medicares.Domain.Entities.Personnel;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Medicares.Persistence.Context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, ICurrentUserService currentUserService) : IdentityDbContext<ApplicationUser, Role, Guid>(options)
{
    public Guid? CurrentOwnerId { get; set; }
    public DbSet<Owner> Owners { get; set; }
    
    public DbSet<Address> Address { get; set; }
    public DbSet<State> State { get; set; }
    
    public DbSet<Store> Stores { get; set; }
    public DbSet<StoreStaff> StoreStaffs { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        foreach (Type clr in builder.Model.GetEntityTypes().Select(x => x.ClrType))
        {
            bool isDeleteEntity = typeof(IDeleteEntity).IsAssignableFrom(clr);
            bool isOwnerEntity = typeof(IOwnerEntity).IsAssignableFrom(clr);

            // Only apply filter if entity implements at least one of the interfaces
            if (isDeleteEntity || isOwnerEntity)
            {
                MethodInfo? methodInfo = typeof(ApplicationDbContext)
                    .GetMethod(nameof(SetCombinedFilter), BindingFlags.Public | BindingFlags.Instance);
                MethodInfo generic = methodInfo!.MakeGenericMethod(clr);

                object[] args = [builder, isDeleteEntity, isOwnerEntity];
                generic.Invoke(this, args);
            }
        }
    }

    public void SetCombinedFilter<TEntity>(ModelBuilder builder, bool applyDeleteFilter, bool applyOwnerFilter)
        where TEntity : class
    {
        Expression<Func<TEntity, bool>>? filter = null;

        if (applyDeleteFilter && applyOwnerFilter)
        {
            // Entity implements both interfaces
            filter = e => (!((IDeleteEntity)e).IsDeleted)
                       && ((IOwnerEntity)e).OwnerId == currentUserService.OwnerId;
        }
        else if (applyDeleteFilter)
        {
            // Only soft delete filter
            filter = e => !((IDeleteEntity)e).IsDeleted;
        }
        else if (applyOwnerFilter)
        {
            // Only owner filter
            filter = e => ((IOwnerEntity)e).OwnerId == currentUserService.OwnerId;
        }

        if (filter != null)
        {
            builder.Entity<TEntity>().HasQueryFilter(filter);
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        DateTime now = DateTime.UtcNow;
        Guid? userId = currentUserService.UserId;

        foreach (Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entityEntry in ChangeTracker.Entries())
        {
            if (entityEntry.Entity is IAuditableEntity auditEntity)
            {
                switch (entityEntry.State)
                {
                    case EntityState.Added:
                        auditEntity.CreatedAt = now;
                        auditEntity.CreatedBy = userId;
                        if (entityEntry.Entity is IOwnerEntity ownerEntity)
                           
                            ownerEntity.OwnerId = CurrentOwnerId ?? currentUserService.OwnerId ?? ownerEntity.OwnerId; 
                          
                        break;

                    case EntityState.Modified:
                        auditEntity.UpdatedAt = now;
                        auditEntity.UpdatedBy = userId;
                        break;

                    case EntityState.Deleted when entityEntry.Entity is IDeleteEntity deleteEntity:
                        {
                            deleteEntity.DeletedBy = userId;
                            deleteEntity.DeletedAt = now;
                            deleteEntity.IsDeleted = true;
                        }
                        auditEntity.UpdatedAt = now;
                        auditEntity.UpdatedBy = userId;
                        entityEntry.State = EntityState.Modified;
                        break;
                }
            }
        }

        return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }
}
