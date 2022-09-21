namespace Avn.Api.Controllers;

public class AuthController : BaseController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>
    /// login user and get a refresh and access token
    /// </summary>
    /// <param name="userLoginDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken)
        => Ok(await _authService.LoginAsync(userLoginDto, cancellationToken));

    /// <summary>
    /// renew a reresh and access token with previous refreshToken
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        => Ok(await _authService.RefreshTokenAsync(refreshToken, cancellationToken));
    
    /// <summary>
    /// logout from system with refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
        => Ok(await _authService.LogoutAsync(refreshToken, cancellationToken));
}