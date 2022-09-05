namespace Avn.Services.Interfaces;

public interface IJwtTokensService : IScopedDependency
{

    /// <summary>
    /// first of all this method gnerates a token
    /// then hashes access tokens and the Adds jwt token to entity 
    /// </summary>
    /// <param name="user">User Model</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse<UserTokenDto>> GenerateUserTokenAsync(User user, string refreshToken = "", CancellationToken cancellationToken = default);

    /// <summary>
    /// Revoke refresh token and all access tokens from user
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IActionResponse> RevokeUserTokensAsync(string refreshToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// get refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<UserJwtToken> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
}
