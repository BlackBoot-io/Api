using Avn.Domain.Dtos.Tokens;
using Avn.Domain.Entities;

namespace Avn.Services.Interfaces;

public class TokenServices : ITokenServices
{

    private readonly ApplicationDBContext _uow;
    private readonly DbSet<Token> _tokens;
    public TokenServices(ApplicationDBContext uow)
    {
        _uow = uow;
        _tokens = _uow.Set<Token>();

    }

    public async Task<IActionResponse<TokenDto>> GetAsync(string uniqueCode, CancellationToken cancellationToken = default)
    {
        var result = await _tokens.Where(x => x.UniqueCode == uniqueCode).Select(row => new TokenDto
        {

        }).FirstOrDefaultAsync(cancellationToken);

        if (result == null)
            return new ActionResponse<TokenDto>(ActionResponseStatusCode.NotFound, AppResource.NotFound);

        return new ActionResponse<TokenDto>(result);

    }
    public async Task<IActionResponse<string>> AddAsync(CreateTokenDto item, CancellationToken cancellationToken = default)
    {
        var model = new Token { };

        await _tokens.AddAsync(model, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<string>(model.UniqueCode);
    }

    public async Task<IActionResponse<IEnumerable<string>>> AddRangeAsync(List<CreateTokenDto> items, CancellationToken cancellationToken = default)
    {
        var models = items.Select(row => new Token { });

        await _tokens.AddRangeAsync(models, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<IEnumerable<string>>(models.Select(x => x.UniqueCode));
    }

    public async Task<IActionResponse<bool>> ConnectWalletAsync(Guid id, string walletAdress, CancellationToken cancellationToken = default)
    {
        var model = await _tokens.FirstOrDefaultAsync(x => x.Id == id && x.OwerWalletAddress == null, cancellationToken);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, AppResource.NotFound);

        model.OwerWalletAddress = walletAdress;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }
    public async Task<IActionResponse<bool>> MintAsync(Guid id, int contractTokenId, CancellationToken cancellationToken = default)
    {
        var model = await _tokens.FirstOrDefaultAsync(x => x.Id == id && x.OwerWalletAddress != null && !x.Mint, cancellationToken);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, AppResource.NotFound);

        model.Mint = true;
        model.ContractTokenId = contractTokenId;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }

    public async Task<IActionResponse<bool>> BurnAsync(Guid id, int contractTokenId, CancellationToken cancellationToken = default)
    {
        var model = await _tokens.FirstOrDefaultAsync(x => x.Id == id && x.OwerWalletAddress != null && x.Mint && x.Burn, cancellationToken);
        if (model == null)
            return new ActionResponse<bool>(ActionResponseStatusCode.NotFound, AppResource.NotFound);

        model.Burn = true;
        model.ContractTokenId = contractTokenId;

        await _uow.SaveChangesAsync(cancellationToken);

        return new ActionResponse<bool>(true);
    }

}
