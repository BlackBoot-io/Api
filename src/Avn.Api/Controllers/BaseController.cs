using Avn.Api.Filters;

namespace Avn.Api.Controllers;

[ApiController, Authorize, ApiResult]
[Route("[controller]/[action]")]
public class BaseController : ControllerBase
{
}
