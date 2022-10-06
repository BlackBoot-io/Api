#nullable disable
using Avn.Data.Context;

namespace Avn.Data.Repository;

public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
{
    protected readonly DbSet<TEntity> _entities;
    public GenericRepo(ApplicationDbContext dbContext) => _entities = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Queryable() => _entities;

    public async Task<TEntity> FindAsync(object[] keyValues, CancellationToken cancellationToken = default) => await _entities.FindAsync(keyValues, cancellationToken);

    public async Task<TEntity> FindAsync(object key, CancellationToken cancellationToken = default) => await _entities.FindAsync(new object[] { key }, cancellationToken);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => await _entities.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => await _entities.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) => _entities.Update(entity);

    public void Remove(TEntity entity) => _entities.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => _entities.RemoveRange(entities);

}