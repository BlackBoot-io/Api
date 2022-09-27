namespace Avn.Services.Implementations;

public class NetworkService : INetworkService
{
    private readonly IAppUnitOfWork _uow;
    public NetworkService(IAppUnitOfWork unitOfWork) => _uow = unitOfWork;

    /// <summary>
    /// Get all network we are supporting
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<Network>>> GetAllAvailableAsync()
          => new ActionResponse<IEnumerable<Network>>(await _uow.NetworkRepo.Queryable().Where(X => X.IsActive).AsNoTracking().ToListAsync());

}

