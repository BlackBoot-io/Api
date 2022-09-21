namespace Avn.Api.Controllers;

public class UserController : BaseController
{
    private readonly IUsersService _userService;

    public UserController(IUsersService userService) => _userService = userService;

    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> SignupAsync(User user, CancellationToken cancellationToken)
        => Ok(await _userService.SignUpAsync(user, cancellationToken));

    /// <summary>
    /// Activate user's email via activation link that has been sent in signUp phase
    /// </summary>
    /// <param name="userId">automatic binded</param>
    /// <param name="uniqueCode">fill from query string</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true/false</returns>
    [HttpPost]
    public async Task<IActionResult> ActivateEmailAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken)
        => Ok(await _userService.ActivateEmailAsync(userId, uniqueCode, cancellationToken));

    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
        => Ok(await _userService.GetCurrentUserAsync(CurrentUserId, cancellationToken));

    /// <summary>
    /// update user's profile such as walletAddress , fullname  and organization's name
    /// </summary>
    /// <param name="userId">automatic binded</param>
    /// <param name="user">sent by client</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UpdateProfileAsync(Guid userId, UserDto user, CancellationToken cancellationToken)
      => Ok(await _userService.UpdateProfileAsync(userId, user, cancellationToken));
}
