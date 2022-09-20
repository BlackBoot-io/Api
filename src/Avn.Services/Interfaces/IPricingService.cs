﻿namespace Avn.Services.Interfaces;

public interface IPricingService
{
    /// <summary>
    /// Get all available pricing model for UI
    /// </summary>
    /// <returns></returns>
    Task<IActionResponse<object>> GetAvailablePricing();
}