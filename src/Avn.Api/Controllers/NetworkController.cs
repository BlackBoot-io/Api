namespace Avn.Api.Controllers;

public class NetworkController : Controller
{
    private readonly INetworkService _networkService;
    public NetworkController(INetworkService networkService)
    {
        _networkService = networkService;

    }
    public async Task<IActionResult> AllAvailableAsync()
        => Ok(await _networkService.GetAllAvailableAsync());
}