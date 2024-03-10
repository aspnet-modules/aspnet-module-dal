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
    /// <summary>
    ///     Добавить CreatedAt, UpdatedAt к объекту
    /// </summary>
    public static EntityTypeBuilder<T> MapAuditable<T>(this EntityTypeBuilder<T> builder)
        where T : class, ICreationTrackable, IModificationTrackable
    {
        builder.Property(x => x.UpdatedAt).HasComment("Обновлен");
        builder.Property(x => x.CreatedAt).HasComment("Создан");
        return builder;
    }

    /// <summary>
    ///     Добавить CreatedAt объекту
    /// </summary>
    public static EntityTypeBuilder<T> MapCreationTrackable<T>(this EntityTypeBuilder<T> builder)
        where T : class, ICreationTrackable
    {
        builder.Property(x => x.CreatedAt).HasComment("Создан");
        return builder;
    }

    /// <summary>
    ///     Добавить UpdatedAt объекту
    /// </summary>
    public static EntityTypeBuilder<T> MapModificationTrackable<T>(this EntityTypeBuilder<T> builder)
        where T : class, IModificationTrackable
    {
        builder.Property(x => x.UpdatedAt).HasComment("Обновлен");
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