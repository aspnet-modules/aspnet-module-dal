using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNet.Module.Dal.EfCore.Database.Tenant;

/// <summary>
///     Фабрика ключей DbContext о тенанту
/// </summary>
internal class TenantModelCacheKeyFactory : IModelCacheKeyFactory
{
    public object Create(DbContext context, bool designTime) =>
        context is BaseTenantDbContext sharedDbContext && !string.IsNullOrWhiteSpace(sharedDbContext.Tenant)
            ? (context.GetType(), sharedDbContext.Tenant, designTime)
            : (context.GetType(), designTime);
}