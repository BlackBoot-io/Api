namespace Avn.Services.Interfaces;

public interface IApiKeyService : IScopedDependency
{
    /// <summary>
    /// Check if the api key is valid
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>collection of user information </returns>
    Task<IActionResponse<ApiKeyDto>> VerifyAsync(Guid apiKey, CancellationToken cancellationToken = default);
}
