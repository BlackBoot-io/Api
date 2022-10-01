namespace Avn.Services.Interfaces;

public interface ICacheService : IScopedDependency
{
    Task<T> GetAsync<T>(string cacheKey, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string cacheKey, T cacheValue, TimeSpan expiration, CancellationToken cancellationToken = default);
    Task RemoveAsync(string cacheKey, CancellationToken cancellationToken = default);
}
