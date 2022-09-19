namespace Avn.Services.Interfaces;

public class TokensService : ITokensService
{
    private readonly IAppUnitOfWork _uow;

    public TokensService(IAppUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// get a token via link's uniqueCode 
    /// </summary>
    /// <param name="uniqueCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<TokenDto>> GetAsync(string uniqueCode, CancellationToken cancellationToken = default)
    {
        var result = await _uow.TokenRepo.Queryable().Select(row => new TokenDto
        {
            DropName = row.Drop.Name,
            DropCategoryType = row.Drop.CategoryType,
            Network = row.Drop.Network.Name,
            StartDate = row.Drop.StartDate,
            EndDate = row.Drop.EndDate,
            ExpireDate = row.Drop.ExpireDate,
            UniqueCode = row.UniqueCode,
            OwerWalletAddress = row.OwerWalletAddress,
            IsBurned = row.IsBurned,
            IsMinted = row.IsMinted
        }).FirstOrDefaultAsync(x => x.UniqueCode == uniqueCode, cancellationToken);

        if (result is null)
            return new ActionResponse<TokenDto>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        return new ActionResponse<TokenDto>(result);

    }
    /// <summary>
    /// Add a token from Qr delivery Type
    /// this is an internal API
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<string>> AddAsync(CreateTokenDto item, CancellationToken cancellationToken = default)
    {
        var model = new Token
        {
            DropId = item.DropId,
            UniqueCode = Guid.NewGuid().ToString(),
            InsertDate = DateTime.UtcNow,
            Number = (await _uow.TokenRepo.Queryable().Where(x => x.DropId == item.DropId).MaxAsync(x => x.Number)) + 1
        };

        await _uow.TokenRepo.AddAsync(model, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<string>(model.UniqueCode);
    }
    /// <summary>
    /// Add multiple tokens from link Delivery Type
    /// this is an internal API for confirmation a drop
    /// </summary>
    /// <param name="items"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<string>>> AddRangeAsync(List<CreateTokenDto> items, CancellationToken cancellationToken = default)
    {
        var models = items.Select((row, index) => new Token
        {
            DropId = row.DropId,
            UniqueCode = Guid.NewGuid().ToString(),
            InsertDate = DateTime.UtcNow,
            Number = index + 1,

        });

        await _uow.TokenRepo.AddRangeAsync(models, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<IEnumerable<string>>(models.Select(x => x.UniqueCode));
    }

    /// <summary>
    /// Update user's wallet address into token
    /// </summary>
    /// <param name="id"></param>
    /// <param name="walletAdress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> ConnectWalletAsync(Guid id, string walletAdress, CancellationToken cancellationToken = default)
    {
        var model = await _uow.TokenRepo.Queryable().FirstOrDefaultAsync(x => x.Id == id && x.OwerWalletAddress == null, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.OwerWalletAddress = walletAdress;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }
    /// <summary>
    /// update token which is minted by user
    /// then update contract tokenId
    /// </summary>
    /// <param name="id"></param>
    /// <param name="contractTokenId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> MintAsync(Guid id, int contractTokenId, CancellationToken cancellationToken = default)
    {
        var model = await _uow.TokenRepo.Queryable().FirstOrDefaultAsync(x => x.Id == id && x.OwerWalletAddress != null && !x.IsMinted, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.IsMinted = true;
        model.ContractTokenId = contractTokenId;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }

    /// <summary>
    /// burn a token by admin
    /// </summary>
    /// <param name="id">tokenId</param>
    /// <param name="contractTokenId">contract tokenId</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> BurnAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var model = await _uow.TokenRepo.Queryable().FirstOrDefaultAsync(x => x.Id == id && x.OwerWalletAddress != null && x.IsMinted && !x.IsBurned, cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.IsBurned = true;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }
}
