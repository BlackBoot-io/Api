namespace Avn.Api.Controllers
{
    public class PricingController : BaseController
    {
        private readonly IPricingsService _pricingService;
        public PricingController(IPricingsService pricingService) => _pricingService = pricingService;

        /// <summary>
        /// Get all available Blockchain networks which we are support
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAvailablePricing(CancellationToken cancellationToken = default)
            => Ok(await _pricingService.GetAvailablePricing(cancellationToken));
    }
}