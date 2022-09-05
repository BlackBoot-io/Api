using Avn.Data.Repository;
using Avn.Domain.Entities;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Avn.Data.UnitofWork;

public partial class AppUnitOfWork
{
    #region Auth
    public IGenericRepo<User> UserRepo => _dbContext.GetService<IGenericRepo<User>>();
    public IGenericRepo<UserJwtToken> UserJwtTokenRepo => _dbContext.GetService<IGenericRepo<UserJwtToken>>();
    #endregion
    #region Base
    public IGenericRepo<Drop> DropRepo => _dbContext.GetService<IGenericRepo<Drop>>();
    public IGenericRepo<Token> TokenRepo => _dbContext.GetService<IGenericRepo<Token>>();
    public IGenericRepo<Project> ProjectRepo => _dbContext.GetService<IGenericRepo<Project>>();
    public IGenericRepo<Network> NetworkRepo =>_dbContext.GetService<IGenericRepo<Network>>();
    #endregion
}

