namespace Avn.Services.Interfaces;

public interface IUsersService : IScopedDependency
{
    /// <summary>
    /// SignUp User
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> SignUpAsync(UserSignUpDto user, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update User Profile
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<Guid>> UpdateProfileAsync(Guid userId, UpdateUserDto userDto, CancellationToken cancellationToken = default);

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

    /// <summary>
    /// Resend Code fot email activation
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> ResendEmailActivationCode(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// enable Lockout 
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse> LockAsync(Guid userId, DateTime endDateUTC, CancellationToken cancellationToken = default);

    /// <summary>
    /// disable Lockout
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse> UnLockAsync(Guid userId, CancellationToken cancellationToken = default);


    /// <summary>
    /// Check Lockout
    /// </summary>
    /// <param name="userId">user Id</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<bool>> CheckLockoutAsync(Guid userId, CancellationToken cancellationToken = default);
}