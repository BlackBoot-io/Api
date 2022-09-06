namespace Avn.Services.Interfaces;
public interface IAuthService : IScopedDependency
{

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
    /// send refresh token in order to renew a new one
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<UserTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
