#nullable disable
using Avn.Data.Context;

namespace Avn.Data.Repository;

public class GenericRepo<TEntity> : IGenericRepo<TEntity> where TEntity : class
{
    protected readonly DbSet<TEntity> _entities;
    public GenericRepo(ApplicationDbContext dbContext) => _entities = dbContext.Set<TEntity>();

    public IQueryable<TEntity> Queryable() => _entities;

    public async Task<TEntity> FindAsync(object[] keyValues, CancellationToken token = default) => await _entities.FindAsync(keyValues, token);

    public async Task<TEntity> FindAsync(object key, CancellationToken token = default) => await _entities.FindAsync(new object[] { key }, token);

    public async Task AddAsync(TEntity entity, CancellationToken token = default) => await _entities.AddAsync(entity, token);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken token = default) => await _entities.AddRangeAsync(entities, token);

    public void Update(TEntity entity) => _entities.Update(entity);

    public void Remove(TEntity entity) => _entities.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => _entities.RemoveRange(entities);

}