namespace Avn.Services.Interfaces;
public interface INetworkService
{
    Task<IActionResponse<List<Network>>> GetAllAvailableAsync();
}
