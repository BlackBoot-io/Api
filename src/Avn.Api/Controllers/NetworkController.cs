namespace Avn.Api.Controllers;

public class NetworkController : BaseController
{
    private readonly INetworksService _networkService;
    public NetworkController(INetworksService networkService) => _networkService = networkService;

    /// <summary>
    /// Get all available Blockchain networks which we are support.
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> AllAvailableAsync(CancellationToken cancellationToken=default)
        => Ok(await _networkService.GetAllAvailableAsync(cancellationToken));
}