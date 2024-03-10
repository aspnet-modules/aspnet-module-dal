using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AspNet.Module.Dal.EfCore.Extensions.Configurations;

/// <summary>
///     https://stackoverflow.com/a/59185869
/// </summary>
[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public static class JsonPropertyConversionExtensions
{
    public class JsonPropertyConversionOptions
    {
        private static IMemoryValueSerializer? _defaultMemoryValueSerializer;
        private IMemoryValueSerializer? _memorySerializer;

        public IMemoryValueSerializer MemorySerializer
        {
            get => _memorySerializer ?? CustomMemorySerializer ?? DefaultMemoryValueSerializer;
            set => _memorySerializer = value;
        }

        public string NpgsqlType { get; set; } = "jsonb";
        public static IMemoryValueSerializer? CustomMemorySerializer { get; set; }

        private static IMemoryValueSerializer DefaultMemoryValueSerializer =>
            _defaultMemoryValueSerializer ??= new SystemJsonMemoryValueSerializer();
    }

    public interface IMemoryValueSerializer
    {
        T? Deserialize<T>(string json) where T : class?;
        string Serialize<T>(T? obj) where T : class?;
    }

    private class SystemJsonMemoryValueSerializer : IMemoryValueSerializer
    {
        public T? Deserialize<T>(string json) where T : class? => JsonSerializer.Deserialize<T>(json);

        public string Serialize<T>(T? obj) where T : class? => JsonSerializer.Serialize(obj);
    }

    /// <summary>
    ///     Сделать столбец в формате sonb
    /// </summary>
    public static PropertyBuilder<T> HasJsonConversion<T>(this PropertyBuilder<T> propertyBuilder,
        DatabaseFacade databaseFacade, Action<JsonPropertyConversionOptions>? configure = null) where T : class?
    {
        var options = new JsonPropertyConversionOptions();
        configure?.Invoke(options);

        // in memory
        if (!databaseFacade.IsNpgsql())
        {
            return InMemoryJson(propertyBuilder, options);
        }

        propertyBuilder.HasColumnType(options.NpgsqlType);

        return propertyBuilder;
    }

    private static PropertyBuilder<T> InMemoryJson<T>(PropertyBuilder<T> propertyBuilder,
        JsonPropertyConversionOptions options) where T : class?
    {
        var valueSerializer = options.MemorySerializer;

        var valueConverter = new ValueConverter<T, string>
        (
            v => valueSerializer.Serialize(v),
            v => valueSerializer.Deserialize<T>(v)!
        );

        var valueComparer = new ValueComparer<T>
        (
            (l, r) => valueSerializer.Serialize(l).Equals(valueSerializer.Serialize(r)),
            v => v is IEquatable<T> ? v.GetHashCode() : valueSerializer.Serialize(v).GetHashCode(),
            v => valueSerializer.Deserialize<T>(valueSerializer.Serialize(v))!
        );

        propertyBuilder.HasConversion(valueConverter);
        propertyBuilder.Metadata.SetValueComparer(valueComparer);
        propertyBuilder.HasColumnType(options.NpgsqlType);

        return propertyBuilder;
    }
}