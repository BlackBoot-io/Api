#nullable disable
using Avn.Data.Context;

namespace Avn.Data.Repository;

public class GenericRepo<TEntity> : BaseEntityQueryable<TEntity>, IGenericRepo<TEntity> where TEntity : class, IEntity
{
    protected readonly DbSet<TEntity> _entities;
    public GenericRepo(ApplicationDbContext dbContext) => _entities = dbContext.Set<TEntity>();

    public void Add(TEntity entity) => _entities.Add(entity);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => await _entities.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => await _entities.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) => _entities.Update(entity);

    public void Remove(TEntity entity) => _entities.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => _entities.RemoveRange(entities);

}