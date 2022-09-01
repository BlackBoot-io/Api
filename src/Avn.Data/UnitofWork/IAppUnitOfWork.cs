using Avn.Data.Repository;
using Avn.Domain.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Avn.Data.UnitofWork;

public interface IAppUnitOfWork : IDisposable, IAsyncDisposable
{
    public ChangeTracker ChangeTracker { get; }
    public DatabaseFacade Database { get; }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    #region Repos
    #region Admin
    public IGenericRepo<User> UserRepo { get; }
    public IGenericRepo<UserJwtToken> UserJwtTokenRepo { get; }
    #endregion
    #region Base
    public IGenericRepo<Drop> EventRepo { get; }
    public IGenericRepo<Token> TokenRepo { get; }
    public IGenericRepo<Project> ProjectRepo { get; }
    public IGenericRepo<Network> NetworkRepo { get; }
    #endregion
    #endregion
}

