using Avn.Data.Context;
using Avn.Shared.Core.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Avn.Data.UnitofWork;

public partial class AppUnitOfWork : IAppUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    public ChangeTracker ChangeTracker => _dbContext.ChangeTracker;
    public DatabaseFacade Database => _dbContext.Database;

    public AppUnitOfWork(ApplicationDbContext dbContext) 
        => _dbContext = dbContext;

    public async Task<SaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new();
        }
        catch (DbUpdateConcurrencyException e)
        {
            return new(SaveChangesResultType.UpdateConcurrencyException, e.Message);
        }
        catch (DbUpdateException e)
        {

            return new(SaveChangesResultType.UpdateException, e.Message);
        }
    }

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