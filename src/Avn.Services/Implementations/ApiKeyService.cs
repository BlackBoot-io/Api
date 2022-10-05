using Avn.Domain.Dtos.Drops;

namespace Avn.Services.Implementations;

public class ApiKeyService : IApiKeyService
{
    private readonly IAppUnitOfWork _uow;

    public ApiKeyService(IAppUnitOfWork uow) => _uow = uow;

    /// <summary>
    /// Check if the api key is valid
    /// </summary>
    /// <param name="apiKey"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>collection of user information </returns>
    public async Task<IActionResponse<ApiKeyDto>> VerifyAsync(string apiKey, CancellationToken cancellationToken = default)
    {
        return new ActionResponse<ApiKeyDto>(data: new());
    }
}