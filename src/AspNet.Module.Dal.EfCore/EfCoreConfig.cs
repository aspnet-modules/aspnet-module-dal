using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global

namespace AspNet.Module.Dal.EfCore;

/// <summary>
///     Настройки EfCore
/// </summary>
public class EfCoreConfig
{
    /// <summary>
    ///     Область жизни регистрированного сервиса
    /// </summary>
    public virtual ServiceLifetime ServiceLifetime { get; init; } =
        ServiceLifetime.Scoped;

    /// <summary>
    ///     Время в БД
    /// </summary>
    public Func<DateTime>? Clock { get; init; }

    /// <summary>
    ///     Конфигурация DataSource
    /// </summary>
    public Action<NpgsqlDataSourceBuilder>? DataSource { get; init; }

    /// <summary>
    ///     Схема для истории миграций
    /// </summary>
    public string? MigrationsHistorySchema { get; init; }

    /// <summary>
    ///     Конфигурация
    /// </summary>
    public Action<NpgsqlDbContextOptionsBuilder>? Npgsql { get; init; }

    /// <summary>
    ///     Конфигурация DbContextOptionsBuilder
    /// </summary>
    public Action<DbContextOptionsBuilder>? Options { get; init; }

    /// <summary>
    ///     Пул контектов
    /// </summary>
    public PoolingOptions? Pooling { get; init; }

    /// <summary>
    ///     Настройка пула
    /// </summary>
    public class PoolingOptions
    {
        /// <summary>
        ///     Размер пула
        /// </summary>
        public int Size { get; init; } = 1024;
    }
}