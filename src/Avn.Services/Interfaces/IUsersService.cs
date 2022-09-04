namespace Avn.Services.Interfaces;

public interface IUsersService : IScopedDependency
{
    /// <summary>
    /// SignUp User
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> SignUpAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Login into system with username and password
    /// </summary>
    /// <param name="userLoginDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<UserTokenDto>> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken = default);

    /// <summary>
    /// LogOut from system
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update User Profile
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> UpdateProfileAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get current user data with userId
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Active user email from verification link
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> ActivateEmailAsync(Guid userId, string uniqueCode, CancellationToken cancellationToken = default);

}