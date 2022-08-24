using Avn.Domain.Dtos.Tokens;

namespace Avn.Services.Interfaces;

public interface ITokensService : IScopedDependency
{
    Task<IActionResponse<TokenDto>> GetAsync(string uniqueCode, CancellationToken cancellationToken = default);
    Task<IActionResponse<string>> AddAsync(CreateTokenDto item, CancellationToken cancellationToken = default);
    Task<IActionResponse<IEnumerable<string>>> AddRangeAsync(List<CreateTokenDto> items, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> ConnectWalletAsync(Guid id, string walletAdress, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> MintAsync(Guid id, int contractTokenId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> BurnAsync(Guid id, int contractTokenId, CancellationToken cancellationToken = default);
}
