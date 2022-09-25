﻿namespace Avn.Services.Interfaces;

public interface ISubscriptionService : IScopedDependency
{
    /// <summary>
    /// Get current subscription model for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IActionResponse<Subscription>> GetCurrentModelAsync(Guid userId, CancellationToken cancellationToken = default);
}
