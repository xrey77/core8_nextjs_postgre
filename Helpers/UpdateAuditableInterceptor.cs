using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace core8_nextjs_postgre.Helpers
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        DateTime? UpdatedAt { get; set; }
    }

    public class UpdateAuditableInterceptor : SaveChangesInterceptor
    {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context == null) return base.SavingChangesAsync(eventData, result, cancellationToken);

            var entries = context.ChangeTracker.Entries<IAuditable>();

            foreach (var entry in entries)
            {
                // Use DateTime to match the interface definition
                var now = DateTime.UtcNow;

                if (entry.State == EntityState.Added)
                {
                    // Map to interface property names: CreatedOnUtc
                    entry.Entity.CreatedAt = now;
                }

                if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                {
                    // Map to interface property names: ModifiedOnUtc
                    entry.Entity.UpdatedAt = now;
                }
            }

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
