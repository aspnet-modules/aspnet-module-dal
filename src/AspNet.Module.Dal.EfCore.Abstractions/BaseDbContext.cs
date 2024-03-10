using AspNet.Module.Dal.EfCore.Database.Processors;
using AspNet.Module.Dal.EfCore.Database.Processors.Tracking;
using AspNet.Module.Dal.EfCore.Database.Processors.Transient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AspNet.Module.Dal.EfCore;

/// <summary>
///     Базовый контекст БД
/// </summary>
public abstract class BaseDbContext : DbContext
{
    /// <inheritdoc />
    protected BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    /// <inheritdoc />
    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        var ctx = new DbBeforeSaveEntityProcessorContext();
        OnBeforeSaveChanges(ctx);
        var result = base.SaveChanges(acceptAllChangesOnSuccess);
        return result;
    }

    /// <inheritdoc />
    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = new())
    {
        var ctx = new DbBeforeSaveEntityProcessorContext();
        OnBeforeSaveChanges(ctx);
        var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultSchema("public");
        modelBuilder.HasPostgresExtension("uuid-ossp");
    }

    /// <summary>
    ///     Перед сохранением объекта в БД
    /// </summary>
    protected virtual void OnBeforeSaveChangeEntry(EntityEntry entry, DbBeforeSaveEntityProcessorContext ctx)
    {
        TransientBeforeSaveEntityProcessor.ExecuteEntry(entry);
        TrackingBeforeSaveEntityProcessor.ExecuteEntry(entry);
    }

    /// <summary>
    ///     Перед сохранением в БД
    /// </summary>
    protected virtual void OnBeforeSaveChanges(DbBeforeSaveEntityProcessorContext ctx)
    {
        foreach (var entry in ChangeTracker.Entries())
        {
            OnBeforeSaveChangeEntry(entry, ctx);
        }
    }
}