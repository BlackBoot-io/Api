namespace Avn.Api.Controllers;

public class TokenController : BaseController
{
    private readonly ITokensService _tokensService;

    public TokenController(ITokensService tokensService) => _tokensService = tokensService;



    /// <summary>
    /// get all Minted Token for Specific Wallet address
    /// </summary>
    /// <param name="walletAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/Token/Scan/{walletAddress}")]
    public async Task<IActionResult> GetAllAsync(string walletAddress, CancellationToken cancellationToken)
        => Ok(await _tokensService.GetAllAsync(walletAddress, cancellationToken));

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
    /// check if the walletAddress has the drop then return the token detail
    /// </summary>
    /// <param name="walletAddress"></param>
    /// <param name="dropCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("/Token/Scan/{walletAddress}/{dropCode}")]
    public async Task<IActionResult> GetAsync(string walletAddress, Guid dropCode, CancellationToken cancellationToken)
        => Ok(await _tokensService.GetAsync(walletAddress, dropCode, cancellationToken));

    /// <summary>
    /// Update user's wallet address into token
    /// </summary>
    /// <param name="id"></param>
    /// <param name="walletAdress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ConnectWalletAsync([FromBody] ConnectWalletDto item, CancellationToken cancellationToken)
        => Ok(await _tokensService.ConnectWalletAsync(item, cancellationToken));

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