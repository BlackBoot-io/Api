using Avn.Shared.Utilities;

namespace Avn.Services.Interfaces;

public class TokensService : ITokensService
{
    private readonly IAppUnitOfWork _uow;

    public TokensService(IAppUnitOfWork uow) => _uow = uow;


    /// <summary>
    /// get all Minted Token for Specific Wallet address
    /// </summary>
    /// <param name="walletAddress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<IEnumerable<object>>> GetAllAsync(string walletAddress, CancellationToken cancellationToken = default)
    {
        var result = await _uow.TokenRepo.Queryable()
            .Where(x => x.IsMinted && x.OwnerWalletAddress == walletAddress)
            .Select(row => new  
            {
                Token = new
                {
                    row.Id,
                    row.UniqueCode,
                    row.OwnerWalletAddress,
                    row.IsBurned,
                    row.IsMinted,
                },
                Drop = new
                {
                    row.Drop.Id,
                    row.Drop.Name,
                    row.Drop.CategoryType,
                    Network = row.Drop.Network.Name,
                    row.Drop.StartDate,
                    row.Drop.EndDate,
                    row.Drop.ExpireDate,
                    row.Drop.Code
                }
            }).ToListAsync(cancellationToken);

        return new ActionResponse<IEnumerable<object>>(result);
    }

    /// <summary>
    /// get a token via link's uniqueCode 
    /// </summary>
    /// <param name="uniqueCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<object>> GetAsync(string uniqueCode, CancellationToken cancellationToken = default)
    {
        var result = await _uow.TokenRepo.Queryable().Select(row => new
        {
            Token = new
            {
                row.Id,
                row.UniqueCode,
                row.OwnerWalletAddress,
                row.IsBurned,
                row.IsMinted,
            },
            Drop = new
            {
                row.Drop.Id,
                row.Drop.Name,
                row.Drop.CategoryType,
                Network = row.Drop.Network.Name,
                row.Drop.StartDate,
                row.Drop.EndDate,
                row.Drop.ExpireDate,
                row.Drop.Code
            }
        }).FirstOrDefaultAsync(x => x.Token.UniqueCode == uniqueCode, cancellationToken);

        if (result is null)
            return new ActionResponse<object>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        return new ActionResponse<object>(result);

    }

    /// <summary>
    /// get the token for specific drop and walleraddress
    /// </summary>
    /// <param name="walletAddress"></param>
    /// <param name="dropCode"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<TokenDto>> GetAsync(string walletAddress, Guid dropCode, CancellationToken cancellationToken = default)
    {
        var result = await _uow.TokenRepo.Queryable().Select(row => new TokenDto
        {
            //Token
            TokenId = row.Id,
            UniqueCode = row.UniqueCode,
            OwerWalletAddress = row.OwnerWalletAddress,
            IsBurned = row.IsBurned,
            IsMinted = row.IsMinted,

            //Drop
            DropId = row.Drop.Id,
            DropCode = row.Drop.Code,
            DropName = row.Drop.Name,
            DropCategoryType = row.Drop.CategoryType,
            Network = row.Drop.Network.Name,
            StartDate = row.Drop.StartDate,
            EndDate = row.Drop.EndDate,
            ExpireDate = row.Drop.ExpireDate

        }).FirstOrDefaultAsync(x => x.OwerWalletAddress == walletAddress && x.DropCode == dropCode, cancellationToken);

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
            UniqueCode = RandomStringGenerator.Generate(15),
            InsertDate = DateTime.UtcNow,
            OwnerWalletAddress = string.Empty,
            Number = (await _uow.TokenRepo.Queryable()
                                          .Where(x => x.DropId == item.DropId)
                                          .MaxAsync(x => x.Number, cancellationToken: cancellationToken)) + 1
        };

        _uow.TokenRepo.Add(model);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<string>(ActionResponseStatusCode.ServerError, dbResult.Message);

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
            UniqueCode = RandomStringGenerator.Generate(15),
            InsertDate = DateTime.UtcNow,
            OwnerWalletAddress = "",
            Number = index + 1,
        });

        await _uow.TokenRepo.AddRangeAsync(models, cancellationToken);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<IEnumerable<string>>(ActionResponseStatusCode.ServerError, dbResult.Message);

        return new ActionResponse<IEnumerable<string>>(models.Select(x => x.UniqueCode));
    }

    /// <summary>
    /// Update user's wallet address into token
    /// </summary>
    /// <param name="id"></param>
    /// <param name="walletAdress"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> ConnectWalletAsync(ConnectWalletDto item, CancellationToken cancellationToken = default)
    {
        var model = await _uow.TokenRepo.Queryable().FirstOrDefaultAsync(x => x.Id == item.Id && string.IsNullOrEmpty(x.OwnerWalletAddress), cancellationToken);
        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.OwnerWalletAddress = item.WalletAdress;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

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
        var model = await _uow.TokenRepo.Queryable()
                              .FirstOrDefaultAsync(x => x.Id == id && !string.IsNullOrEmpty(x.OwnerWalletAddress) && !x.IsMinted, cancellationToken);

        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.IsMinted = true;
        model.ContractTokenId = contractTokenId;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

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
        var model = await _uow.TokenRepo.Queryable()
                              .FirstOrDefaultAsync(x => x.Id == id && x.IsMinted && !x.IsBurned, cancellationToken);

        if (model is null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, BusinessMessage.NotFound);

        model.IsBurned = true;

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<bool>(ActionResponseStatusCode.ServerError, dbResult.Message);

        return new ActionResponse<bool>(true);
    }


}