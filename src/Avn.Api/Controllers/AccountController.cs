using Avn.Domain.Dtos.Users;

namespace Avn.Api.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService) => _accountService = accountService;

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> SignupAsync(User user, CancellationToken cancellationToken)
        => Ok(await _accountService.SignupAsync(user, cancellationToken));

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken)
        => Ok(await _accountService.LoginAsync(userLoginDto, cancellationToken));

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        => Ok(await _accountService.RefreshTokenAsync(refreshToken, cancellationToken));

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> VerifyAsync(Guid userId, string code, CancellationToken cancellationToken)
        => Ok(await _accountService.VerifyAsync(userId, code, cancellationToken));

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> ResendOtpAsync(Guid userId, CancellationToken cancellationToken)
        => Ok(await _accountService.ResendOtpAsync(userId, cancellationToken));

    [HttpPost]
    public async Task<IActionResult> UpdateEmailAsync(Guid userId, string email, CancellationToken cancellationToken)
        => Ok(await _accountService.UpdateEmailAsync(userId, email, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        => Ok(await _accountService.GetCurrentUserAsync(userId, cancellationToken));

    [HttpDelete]
    public async Task<IActionResult> LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
        => Ok(await _accountService.LogoutAsync(userId, refreshToken, cancellationToken));
}