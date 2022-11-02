using System.Collections;
using System.Linq.Expressions;

namespace Avn.Data.Repository;

public class BaseEntityQueryable<TEntity> : IOrderedQueryable<TEntity> where TEntity : class, IEntity
{
    public Type ElementType => throw new NotImplementedException();

    public Expression Expression => throw new NotImplementedException();

    public IQueryProvider Provider => throw new NotImplementedException();

    public IEnumerator<TEntity> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
