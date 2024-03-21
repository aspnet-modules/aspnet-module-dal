using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace AspNet.Module.Dal.EfCore.Database;

/// <summary>
///     Настройки <see cref="DbContextOptionsBuilder" />
/// </summary>
public static class NpgsqlDbContextConfigurer
{
    /// <summary>
    ///     Настройки
    /// </summary>
    public static void Configure(DbContextOptionsBuilder builder, string connectionString,
        Action<NpgsqlDataSourceBuilder>? configureDataSource = null,
        Action<NpgsqlDbContextOptionsBuilder>? configureNpgsql = null)
    {
        ConfigureEf(builder);

        var dataSource = new NpgsqlDataSourceBuilder(connectionString);
        configureDataSource?.Invoke(dataSource);
        builder.UseNpgsql(dataSource.Build(), b => ConfigureNpgsql(b, configureNpgsql));
    }

    /// <summary>
    ///     Настройки
    /// </summary>
    public static void ConfigureNpgsql(NpgsqlDbContextOptionsBuilder builder,
        Action<NpgsqlDbContextOptionsBuilder>? configure = null)
    {
        builder.MigrationsHistoryTable("__ef_migrations_history");
        builder.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);
        configure?.Invoke(builder);
    }

    private static void ConfigureEf(DbContextOptionsBuilder builder)
    {
        builder.UseSnakeCaseNamingConvention();
        builder.ConfigureWarnings(c =>
        {
            c.Log((CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning, LogLevel.Debug));
            c.Log((CoreEventId.ContextInitialized, LogLevel.Debug));
            c.Log((CoreEventId.QueryCompilationStarting, LogLevel.Debug));
            c.Log((RelationalEventId.MultipleCollectionIncludeWarning, LogLevel.Debug));
            c.Log((RelationalEventId.CommandExecuting, LogLevel.Debug));
            c.Log((RelationalEventId.CommandExecuted, LogLevel.Debug));
            c.Log((RelationalEventId.CommandCreated, LogLevel.Debug));
            c.Log((RelationalEventId.CommandCreating, LogLevel.Debug));
        });
    }
}