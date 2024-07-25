using AspNet.Module.Common;
using AspNet.Module.Dal.EfCore.Clock;
using AspNet.Module.Dal.EfCore.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace AspNet.Module.Dal.EfCore;

/// <summary>
///     Модуль домена
/// </summary>
/// <typeparam name="TDbContext">Тип БД контекста</typeparam>
public class EfCoreModule<TDbContext> : IAspNetModule
    where TDbContext : DbContext
{
    private readonly EfCoreConfig _config;

    static EfCoreModule()
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
    }

    public EfCoreModule() : this(new EfCoreConfig())
    {
    }

    public EfCoreModule(EfCoreConfig config)
    {
        _config = config;
    }

    public void Configure(AspNetModuleContext ctx)
    {
        if (_config.Clock != null)
        {
            DatabaseClock.CustomClock(_config.Clock);
        }

        var connStr = DbConnectionStr.FromConfiguration(ctx.Configuration);
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connStr);
        _config.DataSource?.Invoke(dataSourceBuilder);
        var dataSource = dataSourceBuilder.Build();

        if (_config.Pooling != null)
        {
            ctx.Services.AddDbContextPool<DbContext, TDbContext>(
                (sp, o) =>
                {
                    ConfigureLogging(o, ctx.Configuration);
                    ConfigureNpgsqlContext(o, dataSource, _config.Npgsql, _config.MigrationsHistorySchema);
                    ConfigureInterceptors(sp, o);
                    _config.Options?.Invoke(o);
                },
                _config.Pooling.Size);
        }
        else
        {
            if (_config.RegisterAsFactory == true)
            {
                ctx.Services.AddDbContextFactory<TDbContext>(
                    (sp, o) =>
                    {
                        ConfigureLogging(o, ctx.Configuration);
                        ConfigureNpgsqlContext(o, dataSource, _config.Npgsql, _config.MigrationsHistorySchema);
                        ConfigureInterceptors(sp, o);
                        _config.Options?.Invoke(o);
                    });
            }
            else
            {
                ctx.Services.AddDbContext<DbContext, TDbContext>(
                    (sp, o) =>
                    {
                        ConfigureLogging(o, ctx.Configuration);
                        ConfigureNpgsqlContext(o, dataSource, _config.Npgsql, _config.MigrationsHistorySchema);
                        ConfigureInterceptors(sp, o);
                        _config.Options?.Invoke(o);
                    });
            }
        }
    }

    private static void ConfigureInterceptors(IServiceProvider sp, DbContextOptionsBuilder o)
    {
        var interceptors = sp.GetServices<ISaveChangesInterceptor>().ToList();
        if (interceptors.Any())
        {
            o.AddInterceptors(interceptors);
        }
    }

    private static void ConfigureLogging(DbContextOptionsBuilder o, IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>("DbContext:DetailedErrors"))
        {
            return;
        }

        o.EnableSensitiveDataLogging();
        o.EnableDetailedErrors();
    }

    private static void ConfigureNpgsqlContext(DbContextOptionsBuilder o, NpgsqlDataSource dataSource, 
        Action<NpgsqlDbContextOptionsBuilder>? configure, string? migrationsHistorySchema) =>
        NpgsqlDbContextConfigurer.Configure(o, dataSource, configure, migrationsHistorySchema);
}