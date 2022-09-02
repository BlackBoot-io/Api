using Microsoft.AspNetCore.Mvc;

namespace Avn.Api.Controllers
{
    public class NetworkController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
