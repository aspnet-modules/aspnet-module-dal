using AspNet.Module.Dal.EfCore.Database.Tenant;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNet.Module.Dal.EfCore;

/// <summary>
///     Базовый контекст Tenant БД
/// </summary>
public abstract class BaseTenantDbContext(DbContextOptions options) : BaseDbContext(options)
{
    /// <summary>
    ///     Тенант
    /// </summary>
    public abstract string? Tenant { get; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        if (!string.IsNullOrWhiteSpace(Tenant))
        {
            optionsBuilder.ReplaceService<IModelCacheKeyFactory, TenantModelCacheKeyFactory>();
        }
    }
}