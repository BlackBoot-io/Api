namespace Avn.Services.Implementations;

public class SubscriptionService : ISubscriptionService
{
    private readonly IAppUnitOfWork _uow;
    public SubscriptionService(IAppUnitOfWork unitOfWork)=> _uow = unitOfWork;
    
    /// <summary>
    /// Get current subscription model for a user
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<IActionResponse<Subscription>> GetCurrentModelAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var subscription = await _uow.SubscriptionRepo.Queryable()
              .Include(X => X.Pricing)
              .Include(X => X.Pricing.NetworkInPricings)
              .FirstOrDefaultAsync(X => X.To >= DateTime.Now, cancellationToken);

        if (subscription is null)
            return new ActionResponse<Subscription>() { IsSuccess = false };

        return new ActionResponse<Subscription>() { IsSuccess = true, Data = subscription };
    }
}
