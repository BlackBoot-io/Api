namespace Avn.Api.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken)
        => Ok(await _authService.LoginAsync(userLoginDto, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        => Ok(await _authService.RefreshTokenAsync(refreshToken, cancellationToken));

    [HttpDelete]
    public async Task<IActionResult> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        => Ok(await _authService.LogoutAsync(refreshToken, cancellationToken));
}