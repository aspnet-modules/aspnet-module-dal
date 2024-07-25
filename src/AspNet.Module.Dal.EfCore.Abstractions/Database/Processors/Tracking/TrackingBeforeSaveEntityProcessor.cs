using AspNet.Module.Dal.EfCore.Clock;
using AspNet.Module.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNet.Module.Dal.EfCore.Database.Processors.Tracking;

/// <summary>
///     Сохранение полей CreatedAt, UpdatedAt, DeletedAt
/// </summary>
public static class TrackingBeforeSaveEntityProcessor
{
    /// <summary>
    ///     Процесс
    /// </summary>
    public static void ExecuteEntry(EntityEntry entry)
    {
        // https://gist.github.com/rozputnii/42275c02dc4ede71ab4bc4eec2252f25
        switch (entry.State)
        {
            // Write creation date
            case EntityState.Added when entry.Entity is ICreationTrackable creationAuditable:
                if (!Equals(creationAuditable.CreatedAt, default(DateTime)))
                {
                    return;
                }

                entry.Property(nameof(ICreationTrackable.CreatedAt)).CurrentValue = DatabaseClock.UtcNow;
                break;


            // Soft delete entity
            case EntityState.Deleted when entry.Entity is ISoftDeletable:
                entry.State = EntityState.Modified;
                entry.Property(nameof(ISoftDeletable.DeletedAt)).CurrentValue = DatabaseClock.UtcNow;
                break;

            // Write update date
            case EntityState.Modified when entry.Entity is IModificationTrackable _:
                var prop = entry.Property(nameof(IModificationTrackable.UpdatedAt));
                if (Equals(prop.OriginalValue, prop.CurrentValue))
                {
                    entry.Property(nameof(IModificationTrackable.UpdatedAt)).CurrentValue = DatabaseClock.UtcNow;
                }

                break;
        }
    }
}