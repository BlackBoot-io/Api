using Avn.Data.Context;
using Avn.Data.Repository;
using Avn.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Avn.Data.UnitofWork;

public partial class AppUnitOfWork : IAppUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public AppUnitOfWork(ApplicationDbContext dbContext) => _dbContext = dbContext;


    public ChangeTracker ChangeTracker => _dbContext.ChangeTracker;

    public DatabaseFacade Database => _dbContext.Database;

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _dbContext.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        _dbContext.Dispose();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await _dbContext.DisposeAsync();
        GC.SuppressFinalize(this);
    }
}
