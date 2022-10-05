namespace Avn.Services.Implementations;

public class NetworksService : INetworksService
{
    private readonly IAppUnitOfWork _uow;
    public NetworksService(IAppUnitOfWork unitOfWork) => _uow = unitOfWork;

    /// <summary>
    /// Get all network we are supporting
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<Network>>> GetAllAvailableAsync(CancellationToken cancellationToken = default)
          => new ActionResponse<IEnumerable<Network>>(await _uow.NetworkRepo.Queryable().Where(X => X.IsActive).AsNoTracking().ToListAsync(cancellationToken));

}

