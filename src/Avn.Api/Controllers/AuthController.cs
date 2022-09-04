namespace Avn.Api.Controllers;

public class AuthController : BaseController
{
    private readonly IUsersService _userService;

    public AuthController(IUsersService userService) => _userService = userService;

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken)
        => Ok(await _userService.LoginAsync(userLoginDto, cancellationToken));

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        => Ok(await _userService.RefreshTokenAsync(refreshToken, cancellationToken));

    [HttpDelete]
    public async Task<IActionResult> LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
        => Ok(await _userService.LogoutAsync(userId, refreshToken, cancellationToken));
}