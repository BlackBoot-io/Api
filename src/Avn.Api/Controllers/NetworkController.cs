namespace Avn.Api.Controllers;

public class NetworkController : BaseController
{
    private readonly INetworkService _networkService;
    public NetworkController(INetworkService networkService) => _networkService = networkService;

    /// <summary>
    /// Get all available Blockchain networks which we are support
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> AllAvailableAsync()
        => Ok(await _networkService.GetAllAvailableAsync());
}