#nullable disable
using Avn.Data.Context;

namespace Avn.Data.Repository;

public class GenericRepo<TEntity> : BaseEntityQueryable<TEntity>, IGenericRepo<TEntity> where TEntity : class, IEntity
{
    public GenericRepo(ApplicationDbContext dbContext)
        : base(dbContext.Set<TEntity>()) { }

    public void Add(TEntity entity) => _entity.Add(entity);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default) => await _entity.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) => await _entity.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity) => _entity.Update(entity);

    public void Remove(TEntity entity) => _entity.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => _entity.RemoveRange(entities);
}