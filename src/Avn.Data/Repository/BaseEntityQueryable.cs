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
        Expression = entity.AsQueryable().Expression;
        Provider = entity.AsQueryable().Provider;
    }

    public IEnumerator<TEntity> GetEnumerator()
        => throw new NotSupportedException();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}