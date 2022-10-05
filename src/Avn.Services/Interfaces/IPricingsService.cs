namespace Avn.Services.Interfaces;

public interface IPricingsService:IScopedDependency
{
    /// <summary>
    /// Get all available pricing model for UI
    /// </summary>
    /// <returns></returns>
    Task<IActionResponse<object>> GetAvailablePricing(CancellationToken cancellationToken=default);
}