using Avn.Data.Repository;
using Avn.Domain.Entities;
using Avn.Shared.Core.Data;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Avn.Data.UnitofWork;

public interface IAppUnitOfWork : IDisposable, IAsyncDisposable
{
    public ChangeTracker ChangeTracker { get; }
    public DatabaseFacade Database { get; }

    public Task<SaveChangesResult> SaveChangesAsync(CancellationToken cancellationToken = default);

    #region Repos
    #region Auth
    public IGenericRepo<User> UserRepo { get; }
    public IGenericRepo<UserJwtToken> UserJwtTokenRepo { get; }
    public IGenericRepo<VerificationCode> VerificationCodeRepo { get; }
    #endregion
    #region Base
    public IGenericRepo<Drop> DropRepo { get; }
    public IGenericRepo<Token> TokenRepo { get; }
    public IGenericRepo<Project> ProjectRepo { get; }
    public IGenericRepo<Network> NetworkRepo { get; }
    public IGenericRepo<Subscription> SubscriptionRepo { get; }
    public IGenericRepo<Pricing> PricingRepo { get; }
    public IGenericRepo<NetworkInPricing> NetworkInPricingRepo { get; }
    #endregion

    #region File
    public IGenericRepo<Attachment> AttachmentRepo { get; }
    #endregion

    #endregion
}