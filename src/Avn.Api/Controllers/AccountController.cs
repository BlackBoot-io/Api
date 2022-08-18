using Avn.Domain.Dtos;
using Avn.Services.Interfaces;

namespace Avn.Api.Controllers;

public class AccountController : BaseController
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService) => _accountService = accountService;

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken)
        => Ok(await _accountService.LoginAsync(userLoginDto, cancellationToken));



    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        => Ok(await _accountService.RefreshTokenAsync(refreshToken, cancellationToken));


    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken)
        => Ok(await _accountService.GetCurrentUserAsync(userId, cancellationToken));

    [HttpDelete]
    public async Task<IActionResult> LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
        => Ok(await _accountService.LogoutAsync(userId, refreshToken, cancellationToken));

}