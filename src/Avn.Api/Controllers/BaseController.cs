using Avn.Api.Filters;
using Avn.Shared.Extentions;

namespace Avn.Api.Controllers;

[ApiController, Authorize, ApiResult]
[Route("[controller]/[action]")]
public class BaseController : ControllerBase
{
    public Guid CurrentUserId => HttpContext?.User?.Identity?.GetUserIdAsGuid() ?? Guid.Empty;
}