namespace Avn.Services.Interfaces;

public interface IAccountService : IScopedDependency
{
    Task<IActionResponse<UserTokenDto>> SignupAsync(User user, CancellationToken cancellationToken = default);
    Task<IActionResponse<UserTokenDto>> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken = default);
    Task<IActionResponse<UserTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<IActionResponse> LogoutAsync(Guid userId, string refreshToken, CancellationToken cancellationToken = default);
    Task<IActionResponse<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> VerifyAsync(Guid userId, string code, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> ResendOtpAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IActionResponse<bool>> UpdateEmailAsync(Guid userId, string email, CancellationToken cancellationToken = default);
}