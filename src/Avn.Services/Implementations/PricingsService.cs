namespace Avn.Services.Implementations;
public class PricingsService : IPricingsService
{
    private readonly IAppUnitOfWork _uow;
    public PricingsService(IAppUnitOfWork uow)
           => _uow = uow;

    /// <summary>
    /// Get all available pricing model for UI
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResponse<object>> GetAvailablePricing()
           => new ActionResponse<object>(await _uow.NetworkInPricingRepo.Queryable()
                        .Include(X => X.Pricing)
                        .Include(X => X.Network)
                        .Where(X => X.Pricing.IsActive)
                        .AsNoTracking()
                        .Select(X => new
                        {
                            X.Pricing.Name,
                            X.Pricing.PriorityTicketsSupport,
                            X.Pricing.PublicDocumentation,
                            X.Pricing.RequestsPerDay,
                            X.Pricing.RequestsPerSecond,
                            X.Pricing.UsdtAmount,
                            X.Pricing.DiscountForYearlySubscription,
                            X.Pricing.HasTransactionWages,
                            Networks = new
                            {
                                X.Network.Name,
                                X.Network.Type
                            }
                        })
                        .ToListAsync()
           );
}