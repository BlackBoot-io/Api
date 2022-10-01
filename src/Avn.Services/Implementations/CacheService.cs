using EasyCaching.Core;

namespace Avn.Services.Implementations;

public class CacheService : ICacheService
{
    private readonly IEasyCachingProvider _cachingProvider;

    public CacheService(IEasyCachingProviderFactory factory) => _cachingProvider = factory.GetCachingProvider("inMemoryCache");

    public async Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default)
        => (await _cachingProvider.GetAsync<T>(cacheKey, cancellationToken)).Value;

    public async Task SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration, CancellationToken cancellationToken = default)
        => await _cachingProvider.SetAsync(cacheKey, cacheValue, expiration, cancellationToken);

    public async Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default)
        => await _cachingProvider.RemoveAsync(cacheKey, cancellationToken);

}
