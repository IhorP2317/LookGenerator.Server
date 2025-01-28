using LookGenerator.Application.Abstractions;
using LookGenerator.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace LookGenerator.Persistence.Data.Interceptors ;

    public class AuditingSaveChangesInterceptor(ICurrentUserService currentUserService)
        : SaveChangesInterceptor
    {
        public override InterceptionResult<int> SavingChanges(
            DbContextEventData eventData,
            InterceptionResult<int> result)
        {
            AuditEntities(eventData);
            return base.SavingChanges(eventData, result);
        }


        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            AuditEntities(eventData);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void AuditEntities(DbContextEventData eventData)
        {
            var dbContext = eventData.Context;
            if (dbContext == null) return;

            foreach (var entry in dbContext.ChangeTracker.Entries())
            {
                if (entry.Entity is not BaseEntity auditable)
                    continue;

                if (entry.State is not (EntityState.Added or EntityState.Modified)) continue;
                var currentUserId = currentUserService.UserId;
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedAt = DateTime.UtcNow;
                    auditable.CreatedBy =
                        string.IsNullOrEmpty(currentUserId) ? null : Guid.Parse(currentUserId);
                }
                else
                {
                    auditable.ModifiedAt = DateTime.UtcNow;
                    auditable.ModifiedBy =
                        string.IsNullOrEmpty(currentUserId) ? null : Guid.Parse(currentUserId);
                }
            }
        }
    }