namespace Avn.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    /// <summary>
    /// Get current subscription model for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public Task<IActionResponse<Subscription>> GetCurrentModelAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}
