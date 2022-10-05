namespace Avn.Services.Interfaces;

public interface ITokensService : IScopedDependency
{


    /// <summary>
    /// get all Minted Token for Specific Wallet address
    /// </summary>
    /// <param name="walletAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<TokenDto>>> GetAllAsync(string walletAddress, CancellationToken cancellationToken = default);

    /// <summary>
    /// get a token via link's uniqueCode 
    /// </summary>
    /// <param name="uniqueCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<TokenDto>> GetAsync(string uniqueCode, CancellationToken cancellationToken = default);


    /// <summary>
    /// get the token for specific drop and walleraddress
    /// </summary>
    /// <param name="walletAddress"></param>
    /// <param name="dropCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<TokenDto>> GetAsync(string walletAddress, Guid dropCode, CancellationToken cancellationToken = default);



    /// <summary>
    /// Add a token from Qr delivery Type
    /// this is an internal API
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<string>> AddAsync(CreateTokenDto item, CancellationToken cancellationToken = default);


    /// <summary>
    /// Add multiple tokens from link Delivery Type
    /// this is an internal API for confirmation a drop
    /// </summary>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<IEnumerable<string>>> AddRangeAsync(List<CreateTokenDto> items, CancellationToken cancellationToken = default);



    /// <summary>
    /// Update user's wallet address into token
    /// </summary>
    /// <param name="id"></param>
    /// <param name="walletAdress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> ConnectWalletAsync(ConnectWalletDto item, CancellationToken cancellationToken = default);

    /// <summary>
    /// update token which is minted by user
    /// then update contract tokenId
    /// </summary>
    /// <param name="id"></param>
    /// <param name="contractTokenId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> MintAsync(Guid id, int contractTokenId, CancellationToken cancellationToken = default);

    /// <summary>
    /// burn a token by admin
    /// </summary>
    /// <param name="id">tokenId</param>
    /// <param name="contractTokenId">contract tokenId</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> BurnAsync(Guid id, CancellationToken cancellationToken = default);
}
