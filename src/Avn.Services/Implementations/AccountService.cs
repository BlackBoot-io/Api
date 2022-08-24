using Avn.Domain.Dtos.Users;
using Avn.Domain.Enums;
using Avn.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace Avn.Services.Implementations;

public class AccountService : IAccountService
{

    private readonly IUsersService _userService;
    private readonly IUserJwtTokensService _userTokenService;
    private readonly IJwtTokenFactory _tokenFactoryService;
    private readonly IConfiguration _configuration;

    public AccountService(IUsersService userService,
                          IUserJwtTokensService userTokenService,
                          IJwtTokenFactory tokenFactoryService,
                          IConfiguration configuration)
    {
        _userService = userService;
        _userTokenService = userTokenService;
        _tokenFactoryService = tokenFactoryService;
        _configuration = configuration;
    }

    public async Task<IActionResponse<UserTokenDto>> LoginAsync(UserLoginDto item, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetByEmailAsync(item.Email, cancellationToken);
        if (user.Data is null) return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        var checkPasswordResult = _userService.CheckPassword(user.Data, item.Password, cancellationToken);
        if (!checkPasswordResult.Data) return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        var usertokens = await GenerateTokenAsync(user.Data.UserId, cancellationToken);
        await _userTokenService.AddUserTokenAsync(user.Data.UserId, usertokens.AccessToken, usertokens.RefreshToken, cancellationToken);
        return new ActionResponse<UserTokenDto>(usertokens);
    }
    public async Task<IActionResponse> LogoutAsync(Guid userId, string refreshtoken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshtoken))
            return new ActionResponse(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        await _userTokenService.RevokeUserTokensAsync(userId, refreshtoken, cancellationToken);
        return new ActionResponse();
    }
    public async Task<IActionResponse<UserTokenDto>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var refreshTokenModel = await _userTokenService.GetRefreshTokenAsync(refreshToken, cancellationToken);
        if (refreshTokenModel.Data is null)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.NotFound, AppResource.InvalidUser);

        var usertokens = await GenerateTokenAsync(refreshTokenModel.Data.UserId.Value, cancellationToken);
        await _userTokenService.AddUserTokenAsync(refreshTokenModel.Data.UserId.Value, usertokens.AccessToken, usertokens.RefreshToken, cancellationToken);
        return new ActionResponse<UserTokenDto>(usertokens);
    }
    public async Task<IActionResponse<UserDto>> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetAsync(userId, cancellationToken);
        return new ActionResponse<UserDto>(new UserDto
        {
            Email = user.Data.Email,
            FullName = user.Data.FullName,
            UserType = user.Data.UserType,
            OrganizationName = user.Data.OrganizationName,
            WalletAddress = user.Data.WalletAdress,
            UserId = user.Data.UserId,
            EmailIsApproved=user.Data.EmailIsApproved,
            IsActive= user.Data.IsActive
        });
    }


    private async Task<UserTokenDto> GenerateTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        var user = await _userService.GetAsync(userId, cancellationToken);
        var accessToken = _tokenFactoryService.CreateToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,user.Data.UserId.ToString()),
                new Claim(ClaimTypes.Email,user.Data.Email)
            }, JwtTokenType.AccessToken);

        var refreshToken = _tokenFactoryService.CreateToken(new List<Claim>
            {
               new Claim("AccessToken",accessToken.Data.Token)

            }, JwtTokenType.RefreshToken);
        UserTokenDto result = new()
        {
            AccessToken = accessToken.Data.Token,
            AccessTokenExpireTime = DateTimeOffset.UtcNow.AddMinutes(accessToken.Data.TokenExpirationMinutes),
            RefreshToken = refreshToken.Data.Token,
            RefreshTokenExpireTime = DateTimeOffset.UtcNow.AddMinutes(refreshToken.Data.TokenExpirationMinutes),
            User = new UserDto
            {
                Email = user.Data.Email,
                FullName = user.Data.FullName,
                UserType = user.Data.UserType,
                OrganizationName = user.Data.OrganizationName,
                WalletAddress = user.Data.WalletAdress,
                UserId = user.Data.UserId,
                EmailIsApproved = user.Data.EmailIsApproved,
                IsActive = user.Data.IsActive
            }
        };

        return result;
    }




}