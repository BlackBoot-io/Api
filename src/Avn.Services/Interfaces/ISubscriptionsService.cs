using Avn.Domain.Dtos.Subscriptions;

namespace Avn.Services.Interfaces;

public interface ISubscriptionsService : IScopedDependency
{
    /// <summary>
    /// Get current subscription model for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IActionResponse<Subscription>> GetCurrentModelAsync(Guid userId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Add User Subscription
    /// this is an internal API
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<int>> AddAsync(CreateSubscriptionDto item, CancellationToken cancellationToken = default);

}
