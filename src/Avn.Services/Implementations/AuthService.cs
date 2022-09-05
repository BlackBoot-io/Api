using Avn.Data.UnitofWork;

namespace Avn.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IJwtTokensService _jwtTokensService;

    public AuthService(IAppUnitOfWork uow, IJwtTokensService jwtTokensService)
    {
        _uow = uow;
        _jwtTokensService = jwtTokensService;
    }

    /// <summary>
    /// check username and password then create jwt token
    /// </summary>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UserTokenDto>> LoginAsync(UserLoginDto item, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo.GetAll()
                                      .FirstOrDefaultAsync(X => X.Email == item.Email, cancellationToken);

        if (user is null || user.Password != HashGenerator.Hash(item.Password, user.PasswordSalt)) return new ActionResponse<UserTokenDto> { Message = BusinessMessage.InvalidUsernameOrPassword };
        if (!user.IsActive) return new ActionResponse<UserTokenDto> { Message = BusinessMessage.AccountIsDeActive };

        return await _jwtTokensService.GenerateUserTokenAsync(user, cancellationToken: cancellationToken);
    }

    public async Task<IActionResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
         => await _jwtTokensService.RevokeUserTokensAsync(refreshToken, cancellationToken);

    public async Task<IActionResponse<UserTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenModel = await _jwtTokensService.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (refreshTokenModel is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, BusinessMessage.InvalidUser);

        var user = await _uow.UserRepo.GetAll()
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(X => X.UserId == refreshTokenModel.UserId, cancellationToken);
        if (user is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, BusinessMessage.InvalidUser);

        return await _jwtTokensService.GenerateUserTokenAsync(user, refreshToken, cancellationToken: cancellationToken);
    }
}
