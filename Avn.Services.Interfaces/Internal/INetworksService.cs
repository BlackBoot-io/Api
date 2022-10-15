namespace Avn.Services.Interfaces;
public interface INetworksService: IScopedDependency
{
    /// <summary>
    /// Get all network we are supporting
    /// </summary>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<Network>>> GetAllAvailableAsync(CancellationToken cancellationToken=default);
}
