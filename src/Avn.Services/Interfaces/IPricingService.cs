namespace Avn.Services.Interfaces;

public interface IPricingService
{
    /// <summary>
    /// Get all available pricing model
    /// </summary>
    /// <returns></returns>
    Task<IActionResponse<List<object>>> GetAvailablePricing();
}