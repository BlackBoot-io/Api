namespace Avn.Api.Controllers;

public class TokenController : Controller
{
    private readonly ITokensService _tokensService;

    public TokenController(ITokensService tokensService) => _tokensService = tokensService;


    /// <summary>
    /// Get Token By Token UniqueCode
    /// </summary>
    /// <param name="uniqueCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetAsync(string uniqueCode, CancellationToken cancellationToken)
        => Ok(await _tokensService.GetAsync(uniqueCode, cancellationToken));

    /// <summary>
    /// Update user's wallet address into token
    /// </summary>
    /// <param name="id"></param>
    /// <param name="walletAdress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ConnectWalletAsync(Guid id, string walletAdress, CancellationToken cancellationToken)
        => Ok(await _tokensService.ConnectWalletAsync(id, walletAdress, cancellationToken));
    
    /// <summary>
    /// Update user's wallet address into token
    /// </summary>
    /// <param name="id"></param>
    /// <param name="contractTokenId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> MintAsync(Guid id, int contractTokenId, CancellationToken cancellationToken)
        => Ok(await _tokensService.MintAsync(id, contractTokenId, cancellationToken));

    /// <summary>
    /// Update user's wallet address into token
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> BurnAsync(Guid id, CancellationToken cancellationToken)
        => Ok(await _tokensService.BurnAsync(id, cancellationToken));
}