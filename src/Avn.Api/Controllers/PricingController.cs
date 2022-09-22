namespace Avn.Api.Controllers
{
    public class PricingController : BaseController
    {
        private readonly IPricingService _pricingService;
        public PricingController(IPricingService pricingService) => _pricingService = pricingService;

        /// <summary>
        /// Get all available Blockchain networks which we are support
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAvailablePricing()
            => Ok(await _pricingService.GetAvailablePricing());
    }
}
