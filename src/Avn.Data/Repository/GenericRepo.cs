using Avn.Data.Context;

namespace Avn.Data.Repository;

public sealed class GenericRepo<TEntity> : BaseEntityQueryable<TEntity>, IGenericRepo<TEntity> where TEntity : class, IEntity
{
    public GenericRepo(ApplicationDbContext dbContext)
        : base(dbContext.Set<TEntity>()) { }


    public TEntity? Find(params object?[]? keyValues)
             => _entity.Find(keyValues);


    public async ValueTask<TEntity?> FindAsync(params object?[]? keyValues)
        => await _entity.FindAsync(keyValues);

    public void Add(TEntity entity)
        => _entity.Add(entity);

    public async Task AddAsync(TEntity entity)
     => await _entity.AddAsync(entity);

    public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        => await _entity.AddAsync(entity, cancellationToken);

    public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        => await _entity.AddRangeAsync(entities, cancellationToken);

    public void Update(TEntity entity)
        => _entity.Update(entity);

    public void Remove(TEntity entity)
        => _entity.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities)
        => _entity.RemoveRange(entities);
}