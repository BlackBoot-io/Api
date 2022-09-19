namespace Avn.Data.Repository;

public interface IGenericRepo<TEntity> where TEntity : class
{
    public IQueryable<TEntity> Queryable();
    public Task<TEntity> FindAsync(object key, CancellationToken token = default);
    public Task<TEntity> FindAsync(object[] keyValues, CancellationToken token = default);
    public Task AddAsync(TEntity entity, CancellationToken token = default);
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default);
    public void Update(TEntity entity);
    public void Remove(TEntity entity);
    public void RemoveRange(IEnumerable<TEntity> entities);
}

