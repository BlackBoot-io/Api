using Avn.Data.Repository;
using Avn.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Avn.Data.UnitofWork;

public partial class AppUnitOfWork
{
    #region Auth
    public IGenericRepo<User> UserRepo => _dbContext.GetService<IGenericRepo<User>>();
    public IGenericRepo<UserJwtToken> UserJwtTokenRepo => _dbContext.GetService<IGenericRepo<UserJwtToken>>();
    public IGenericRepo<VerificationCode> VerificationCodeRepo => _dbContext.GetService<IGenericRepo<VerificationCode>>();
    #endregion
    #region Base
    public IGenericRepo<Drop> DropRepo => _dbContext.GetService<IGenericRepo<Drop>>();
    public IGenericRepo<Token> TokenRepo => _dbContext.GetService<IGenericRepo<Token>>();
    public IGenericRepo<Project> ProjectRepo => _dbContext.GetService<IGenericRepo<Project>>();
    public IGenericRepo<Network> NetworkRepo => _dbContext.GetService<IGenericRepo<Network>>();
    public IGenericRepo<Pricing> PricingRepo => _dbContext.GetService<IGenericRepo<Pricing>>();
    public IGenericRepo<NetworkInPricing> NetworkInPricingRepo => _dbContext.GetService<IGenericRepo<NetworkInPricing>>();
    public IGenericRepo<Subscription> SubscriptionRepo => _dbContext.GetService<IGenericRepo<Subscription>>();

    #endregion
    #region File
    public IGenericRepo<Attachment> AttachmentRepo => _dbContext.GetService<IGenericRepo<Attachment>>();

    #endregion
}

