namespace Avn.Api.Controllers;

public class UserController : BaseController
{
    private readonly IUsersService _userService;

    public UserController(IUsersService userService) => _userService = userService;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost, AllowAnonymous]
    public async Task<IActionResult> SignupAsync([FromBody] UserSignUpDto user, CancellationToken cancellationToken)
        => Ok(await _userService.SignUpAsync(user, cancellationToken));

    /// <summary>
    /// Activate user's email via activation link that has been sent in signUp phase
    /// </summary>
    /// <param name="uniqueCode">fill from query string</param>
    /// <param name="cancellationToken"></param>
    /// <returns>true/false</returns>
    [HttpGet]
    public async Task<IActionResult> ActivateEmailAsync(string uniqueCode, CancellationToken cancellationToken)
        => Ok(await _userService.ActivateEmailAsync(CurrentUserId, uniqueCode, cancellationToken));


    /// <summary>
    /// Resend Verification Code
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> ResendEmailActivationCode()
        => Ok(await _userService.ResendEmailActivationCode(CurrentUserId));

    /// <summary>
    /// Get Current User Data
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
        => Ok(await _userService.GetCurrentUserAsync(CurrentUserId, cancellationToken));

    /// <summary>
    /// update user's profile such as walletAddress , fullname  and organization's name
    /// </summary>
    /// <param name="user">sent by client</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateUserDto user, CancellationToken cancellationToken)
      => Ok(await _userService.UpdateProfileAsync(CurrentUserId, user, cancellationToken));
}
