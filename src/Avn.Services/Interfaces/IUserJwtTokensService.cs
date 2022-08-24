using Avn.Domain.Entities;

namespace Avn.Services.Interfaces;

public interface IUserJwtTokensService : IScopedDependency
{
    Task<IActionResponse> AddUserTokenAsync(Guid userId, string accessToken, string refreshToken, CancellationToken cancellationToken = default);
    Task<IActionResponse> RevokeUserTokensAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> VerifyTokenAsync(Guid userId, string accessToken, CancellationToken cancellationToken = default);
    Task<IActionResponse<UserJwtToken>> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
}
