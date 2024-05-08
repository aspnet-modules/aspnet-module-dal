using AspNet.Module.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AspNet.Module.Dal.EfCore.Extensions.Configurations;

/// <summary>
///     Расширения для <see cref="IModificationTrackable" />,
///     <see cref="ISoftDeletable" />,
///     <see cref="ICreationTrackable" />
/// </summary>
public static class AuditableEntityExtensions
{
    private const string CreationDefaultValue = "now()";
    
    /// <summary>
    ///     Добавить CreatedAt, UpdatedAt к объекту
    /// </summary>
    public static EntityTypeBuilder<T> MapAuditable<T>(this EntityTypeBuilder<T> builder, 
        string creationDefaultValue = CreationDefaultValue) where T : class, ICreationTrackable, IModificationTrackable
    {
        builder.MapModificationTrackable();
        builder.MapCreationTrackable(creationDefaultValue);
        return builder;
    }

    /// <summary>
    ///     Добавить CreatedAt объекту
    /// </summary>
    public static EntityTypeBuilder<T> MapCreationTrackable<T>(this EntityTypeBuilder<T> builder, 
        string defaultValue = CreationDefaultValue) where T : class, ICreationTrackable
    {
        builder.Property(x => x.CreatedAt).HasDefaultValueSql(defaultValue).HasComment("Создан");
        return builder;
    }

    /// <summary>
    ///     Добавить UpdatedAt объекту
    /// </summary>
    public static EntityTypeBuilder<T> MapModificationTrackable<T>(this EntityTypeBuilder<T> builder)
        where T : class, IModificationTrackable
    {
        builder.Property(x => x.UpdatedAt).IsRequired(false).HasComment("Обновлен");
        return builder;
    }

    /// <summary>
    ///     Добавить DeletedAt объекту
    /// </summary>
    public static EntityTypeBuilder<T> MapSoftDeletable<T>(this EntityTypeBuilder<T> builder)
        where T : class, ISoftDeletable
    {
        builder.Property(x => x.DeletedAt).HasComment("Удален");
        return builder;
    }
}