using AspNet.Module.Common;
using AspNet.Module.Dal.EfCore.Audit.Elk.Interceptors;
using AspNet.Module.Dal.EfCore.Audit.Elk.Options;
using AspNet.Module.Dal.EfCore.Audit.Elk.Services;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNet.Module.Dal.EfCore.Audit.Elk;

public class EfCoreAuditElkModule : IAspNetModule
{
    public void Configure(AspNetModuleContext ctx)
    {
        var section = ctx.Configuration.GetRequiredSection(AuditOptions.Key);
        var options = section.Get<AuditOptions>(o => o.BindNonPublicProperties = true);
        if (options is not { Enabled: true })
        {
            return;
        }

        ctx.Services.Configure<AuditOptions>(
            section,
            o => { o.BindNonPublicProperties = true; });
        ctx.Services.AddScoped<ElkAuditService>();
        ctx.Services.AddScoped<ISaveChangesInterceptor, ElkAuditingInterceptor>();
        ctx.Services.AddHttpContextAccessor();
    }
}