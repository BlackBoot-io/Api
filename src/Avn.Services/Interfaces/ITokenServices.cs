namespace Avn.Services.Interfaces;

public interface ITokenServices : IScopedDependency
{
    Task<IActionResponse<object>> GetAsync(object id, CancellationToken cancellationToken = default);
    Task<IActionResponse<object>> AddAsync(object eventId, CancellationToken cancellationToken = default);
    Task<IActionResponse<object>> ConnectWalletAsync(object item, CancellationToken cancellation = default);
    Task<IActionResponse<object>> MintAsync(object item, CancellationToken cancellation = default);
    Task<IActionResponse<object>> BurnAsync(object item, CancellationToken cancellation = default);
}
