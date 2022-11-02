namespace Avn.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IAppUnitOfWork _uow;
    private readonly IJwtTokensService _jwtTokensService;
    private readonly IUsersService _usersService;

    public AuthService(IAppUnitOfWork uow, IJwtTokensService jwtTokensService, IUsersService usersService)
    {
        _uow = uow;
        _jwtTokensService = jwtTokensService;
        _usersService = usersService;
    }



    /// <summary>
    /// check username and password then create jwt token
    /// </summary>
    /// <param name="userLoginDto"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UserTokenDto>> LoginAsync(UserLoginDto userLoginDto, CancellationToken cancellationToken = default)
    {
        var user = await _uow.UserRepo
                                      .FirstOrDefaultAsync(X => X.Email == userLoginDto.Email, cancellationToken);

        if (user is null || user.Password != HashGenerator.Hash(userLoginDto.Password, user.PasswordSalt))
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidUsernameOrPassword);

        var lockoutResult = await _usersService.CheckLockoutAsync(user.UserId, cancellationToken);
        if (lockoutResult is { IsSuccess: true, Data: true })
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.BadRequest, BusinessMessage.AccountIsLockout);

        if (!user.IsActive)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.BadRequest, BusinessMessage.AccountIsDeActive);

        return await _jwtTokensService.GenerateUserTokenAsync(user, cancellationToken: cancellationToken);
    }

    public async Task<IActionResponse> LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
         => await _jwtTokensService.RevokeUserTokensAsync(refreshToken, cancellationToken);

    public async Task<IActionResponse<UserTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenModel = await _jwtTokensService.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (refreshTokenModel is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, BusinessMessage.InvalidUser);

        var user = await _uow.UserRepo
                                      .AsNoTracking()
                                      .FirstOrDefaultAsync(X => X.UserId == refreshTokenModel.UserId, cancellationToken);
        if (user is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, BusinessMessage.InvalidUser);

        var lockoutResult = await _usersService.CheckLockoutAsync(user.UserId, cancellationToken);
        if (lockoutResult is { IsSuccess: true, Data: true })
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.BadRequest, BusinessMessage.AccountIsLockout);

        return await _jwtTokensService.GenerateUserTokenAsync(user, refreshToken, cancellationToken: cancellationToken);
    }
}
