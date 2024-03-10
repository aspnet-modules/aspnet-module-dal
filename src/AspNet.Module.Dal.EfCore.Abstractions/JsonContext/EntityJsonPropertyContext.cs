using System.Linq.Expressions;
using System.Reflection;
using AspNet.Module.Domain.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace AspNet.Module.Dal.EfCore.JsonContext;

/// <inheritdoc />
internal sealed class EntityJsonPropertyContext : IEntityJsonPropertyContext
{
    private readonly DbContext _dbContext;

    /// <summary>
    ///     Создание контекста
    /// </summary>
    public EntityJsonPropertyContext(DbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public void TrackModify<TEntity, TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression)
        where TEntity : class
    {
        var property = GetSimpleMemberName(propertyExpression.GetPropertyAccess());
        _dbContext.Entry(entity).Property(property).IsModified = true;
    }

    private static string GetSimpleMemberName(MemberInfo member)
    {
        var name = member.Name;
        var index = name.LastIndexOf('.');
        return index >= 0 ? name[(index + 1)..] : name;
    }
}