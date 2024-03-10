using System.Linq.Expressions;

namespace AspNet.Module.Dal.EfCore.Queries;

/// <summary>
///     Мульти поиск
///     <see href="https://gist.github.com/princefishthrower/6620fcded6b2600bbd10f4100c55401c#file-exampleusage-cs-L8" />
/// </summary>
public class ExpressionReplacer : ExpressionVisitor
{
    private readonly Func<Expression, Expression> _replacer;

    /// <inheritdoc />
    public ExpressionReplacer(Func<Expression, Expression> replacer)
    {
        _replacer = replacer;
    }

    /// <inheritdoc />
    public override Expression? Visit(Expression? node) => base.Visit(node == null ? node : _replacer(node));

    /// <summary>
    ///     Агрегация запросов
    /// </summary>
    public static Expression<Func<T, TReturn>> Join<T, TReturn>(
        Func<Expression, Expression, BinaryExpression> joiner,
        IReadOnlyCollection<Expression<Func<T, TReturn>>> expressions)
    {
        if (!expressions.Any())
        {
            throw new ArgumentException("No expressions were provided");
        }

        var firstExpression = expressions.First();
        var otherExpressions = expressions.Skip(1);
        var firstParameter = firstExpression.Parameters.Single();
        var otherExpressionsWithParameterReplaced =
            otherExpressions.Select(e => ReplaceParameter(e.Body, e.Parameters.Single(), firstParameter));
        var bodies = new[] { firstExpression.Body }.Concat(otherExpressionsWithParameterReplaced);
        var joinedBodies = bodies.Aggregate(joiner);
        return Expression.Lambda<Func<T, TReturn>>(joinedBodies, firstParameter);
    }

    /// <summary>
    ///     Заменить параметр
    /// </summary>
    private static T ReplaceParameter<T>(T expr, ParameterExpression toReplace, ParameterExpression replacement)
        where T : Expression
    {
        var replacer = new ExpressionReplacer(e => e == toReplace ? replacement : e);
        return (T)replacer.Visit(expr)!;
    }
}