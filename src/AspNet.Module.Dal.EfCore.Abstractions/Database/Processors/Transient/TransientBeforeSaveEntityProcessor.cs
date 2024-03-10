using AspNet.Module.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNet.Module.Dal.EfCore.Database.Processors.Transient;

/// <summary>
///     Установка триггера <see cref="ITransientEntity.Transient" />
/// </summary>
internal static class TransientBeforeSaveEntityProcessor
{
    /// <summary>
    ///     Процесс
    /// </summary>
    public static void ExecuteEntry(EntityEntry entry)
    {
        switch (entry.State)
        {
            case EntityState.Added:
            case EntityState.Modified:
            case EntityState.Detached:
                if (entry.Entity is ITransientEntity { Transient: true } entity)
                {
                    entry.State = EntityState.Added;
                    entity.Transient = false;
                }

                break;
        }
    }
}