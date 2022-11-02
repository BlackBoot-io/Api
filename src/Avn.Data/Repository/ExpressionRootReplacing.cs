using System.Linq.Expressions;

namespace Avn.Data.Repository;

internal class ExpressionRootReplacing<TEntity> : ExpressionVisitor where TEntity : class
{
    private readonly DbSet<TEntity> _newRoot;

    public ExpressionRootReplacing(DbSet<TEntity> newRoot)
        => _newRoot = newRoot;

    protected override Expression VisitConstant(ConstantExpression node)
    {
        if (node.Type.BaseType != null && node.Type.BaseType.IsGenericType &&
            node.Type.BaseType.GetGenericTypeDefinition() == typeof(BaseEntityQueryable<>))
            return _newRoot.AsQueryable().Provider.CreateQuery(_newRoot.AsQueryable().Expression).Expression;

        return node;
    }
}