using System.Linq.Expressions;
using AspNet.Module.Dal.EfCore.JsonContext;
using AspNet.Module.Domain.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Query;

namespace AspNet.Module.Dal.EfCore.Extensions;

/// <summary>
///     Расширения для <see cref="DbContext" />
/// </summary>
public static class DbContextExtensions
{
    /// <summary>
    ///     Добавить глобальный фильтр
    ///     <see href="https://github.com/dotnet/efcore/issues/10275#issuecomment-572915802" />
    /// </summary>
    public static void AddQueryFilter<T>(this EntityTypeBuilder entityTypeBuilder,
        Expression<Func<T, bool>> expression)
    {
        var parameterType = Expression.Parameter(entityTypeBuilder.Metadata.ClrType);
        var expressionFilter = ReplacingExpressionVisitor.Replace(
            expression.Parameters.Single(), parameterType, expression.Body);

        var currentQueryFilter = entityTypeBuilder.Metadata.GetQueryFilter();
        if (currentQueryFilter != null)
        {
            var currentExpressionFilter = ReplacingExpressionVisitor.Replace(
                currentQueryFilter.Parameters.Single(), parameterType, currentQueryFilter.Body);
            expressionFilter = Expression.AndAlso(currentExpressionFilter, expressionFilter);
        }

        var lambdaExpression = Expression.Lambda(expressionFilter, parameterType);
        entityTypeBuilder.HasQueryFilter(lambdaExpression);
    }

    /// <summary>
    ///     Создание контекста для модификации Json свойства
    /// </summary>
    public static IEntityJsonPropertyContext CreateJsonPropertyContext(this DbContext dbContext) =>
        new EntityJsonPropertyContext(dbContext);

    public static async Task EnsureTransaction(this DbContext dbContext, Func<Task> action)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            await action();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public static async Task<T> EnsureTransaction<T>(this DbContext dbContext, Func<Task<T>> func)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync();
        try
        {
            var result = await func();
            await transaction.CommitAsync();

            return result;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}