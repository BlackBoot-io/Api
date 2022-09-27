namespace Avn.Services.Interfaces;

public interface ITeansactionsService : IScopedDependency
{

    /// <summary>
    /// Add a token from Qr delivery Type
    /// this is an internal API
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<string>> AddAsync(CreateTokenDto item, CancellationToken cancellationToken = default);
}
