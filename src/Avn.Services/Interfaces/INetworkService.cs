namespace Avn.Services.Interfaces;
public interface INetworkService: IScopedDependency
{
    /// <summary>
    /// Get all network we are supporting
    /// </summary>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<Network>>> GetAllAvailableAsync();
}
