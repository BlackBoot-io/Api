namespace Avn.Api.Controllers;

public class UserController : BaseController
{
    private readonly IUsersService _userService;

    public UserController(IUsersService userService) => _userService = userService;

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> SignupAsync(User user, CancellationToken cancellationToken)
        => Ok(await _userService.SignUpAsync(user, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> ActivateEmailAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken)
        => Ok(await _userService.ActivateEmailAsync(userId, uniqueCode, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        => Ok(await _userService.GetCurrentUserAsync(userId, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> UpdateProfileAsync(Guid userId, UserDto user, CancellationToken cancellationToken)
      => Ok(await _userService.UpdateProfileAsync(userId, user, cancellationToken));
}
