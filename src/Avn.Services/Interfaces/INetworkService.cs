namespace Avn.Services.Interfaces;
public interface INetworkService
{
    /// <summary>
    /// Get all network we are supporting
    /// </summary>
    /// <returns></returns>
    Task<IActionResponse<List<Network>>> GetAllAvailableAsync();
}
