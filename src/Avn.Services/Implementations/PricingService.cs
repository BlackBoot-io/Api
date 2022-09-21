namespace Avn.Services.Implementations;
public class PricingService : IPricingService
{
    private readonly IAppUnitOfWork _uow;
    public PricingService(IAppUnitOfWork uow)
           => _uow = uow;

    /// <summary>
    /// Get all available pricing model for UI
    /// </summary>
    /// <returns></returns>
    public async Task<IActionResponse<object>> GetAvailablePricing()
           => new ActionResponse<object>
           {
               IsSuccess = true,
               Data = await _uow.PricingRepo.Queryable()
                        .Include(X => X.NetworkInPricings)
                        .ThenInclude(X => X.Network)
                        .Where(X => X.IsActive)
                        .AsNoTracking()
                        .Select(X => new
                        {

                        })
                        .ToListAsync()
           };
}