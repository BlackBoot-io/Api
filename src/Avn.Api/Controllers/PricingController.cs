using Microsoft.AspNetCore.Mvc;

namespace Avn.Api.Controllers
{
    public class PricingController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
