using System.Collections;
using System.Linq.Expressions;

namespace Avn.Data.Repository;

public class BaseEntityQueryable<TEntity> : IOrderedQueryable<TEntity> where TEntity : class, IEntity
{
    public Expression Expression { get; set; }
    public IQueryProvider Provider { get; }
    public Type ElementType => typeof(TEntity);

    public BaseEntityQueryable(DbSet<TEntity> entity)
    {
        Expression = Expression.Constant(this);
        Provider = new QueryProvider<TEntity>(entity);
    }

    public IEnumerator<TEntity> GetEnumerator()
        => Provider.Execute<IEnumerable<TEntity>>(Expression).GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}