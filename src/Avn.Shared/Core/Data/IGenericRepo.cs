namespace Avn.Shared.Core;

public interface IGenericRepo<TEntity> where TEntity : class
{
    public IQueryable<TEntity> Queryable();
    public Task<TEntity> FindAsync(object key, CancellationToken cancellationToken = default);
    public Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default);
    public Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
    public Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);
    public void Update(TEntity entity);
    public void Remove(TEntity entity);
    public void RemoveRange(IEnumerable<TEntity> entities);
}