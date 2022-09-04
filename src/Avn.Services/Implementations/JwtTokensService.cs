#nullable disable
using Avn.Data.UnitofWork;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace BlackBoot.Services.Implementations;

public class JwtTokensService : IJwtTokensService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IAppUnitOfWork _uow;
    private readonly IJwtTokensFactory _jwtTokensFactory;

    public JwtTokensService(IAppUnitOfWork uow, IJwtTokensFactory jwtTokensFactory, IConfiguration configuration)
    {
        _uow = uow;
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        _jwtTokensFactory = jwtTokensFactory;
    }

    #region Private Methods
    /// <summary>
    /// Generate new access and refresh token for user
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    private UserTokenDto GenerateToken(Guid userId, string email)
    {
        var accessToken = _jwtTokensFactory.CreateToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userId.ToString()),
                new Claim(ClaimTypes.Email,email)
            }, JwtTokenType.AccessToken);

        var refreshToken = _jwtTokensFactory.CreateToken(new List<Claim>
            {
               new Claim("AccessToken",accessToken.Data.Token)
            }, JwtTokenType.RefreshToken);

        UserTokenDto result = new()
        {
            AccessToken = accessToken.Data.Token,
            AccessTokenExpireTime = DateTimeOffset.UtcNow.AddMinutes(accessToken.Data.TokenExpirationMinutes),
            RefreshToken = refreshToken.Data.Token,
            RefreshTokenExpireTime = DateTimeOffset.UtcNow.AddMinutes(refreshToken.Data.TokenExpirationMinutes),
        };
        return result;
    }

    /// <summary>
    /// delete all tokens from one refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DeleteTokensWithSameRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return;
        var hashedRefreshToken = HashGenerator.Hash(refreshToken);
        var tokens = await _uow.UserJwtTokenRepo.GetAll()
                                                .Where(t => t.RefreshTokenHash == hashedRefreshToken).ToListAsync(cancellationToken);
        _uow.UserJwtTokenRepo.RemoveRange(tokens);
        await _uow.SaveChangesAsync(cancellationToken);
    }
    #endregion

    /// <summary>
    /// first of all this method gnerates a token
    /// then hashes access tokens and the Adds jwt token to entity 
    /// </summary>
    /// <param name="user">User Model</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<UserTokenDto>> GenerateUserTokenAsync(User user, string refreshToken = "", CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(refreshToken))
            await DeleteTokensWithSameRefreshTokenAsync(refreshToken, cancellationToken);

        var token = GenerateToken(user.UserId, user.Email);
        await _uow.UserJwtTokenRepo.AddAsync(new UserJwtToken
        {
            UserId = user.UserId,
            AccessTokenHash = HashGenerator.Hash(token.AccessToken),
            AccessTokenExpiresTime = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenHash = HashGenerator.Hash(token.RefreshToken),
            RefreshTokenExpiresTime = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes)
        }, cancellationToken);

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.ServerError);

        token.User = new UserDto
        {
            Email = user.Email,
            EmailIsApproved = user.EmailIsApproved,
            FullName = user.FullName,
            OrganizationName = user.OrganizationName,
            UserType = user.Type,
            WalletAddress = user.WalletAddress,
        };
        return new ActionResponse<UserTokenDto>
        {
            IsSuccess = true,
            Data = token
        };
    }

    /// <summary>
    /// Delete all tokens with refresh token on logout
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse> RevokeUserTokensAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(refreshToken))
            return new ActionResponse(ActionResponseStatusCode.BadRequest, AppResource.InvalidPrameter);

        var hashedRefreshToken = HashGenerator.Hash(refreshToken);
        var tokens = await _uow.UserJwtTokenRepo.GetAll()
                                                .Where(t => t.RefreshTokenHash == hashedRefreshToken).ToListAsync(cancellationToken);
        _uow.UserJwtTokenRepo.RemoveRange(tokens);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse(ActionResponseStatusCode.ServerError);

        return new ActionResponse();
    }

    public async Task<UserJwtToken> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var refresheTokenHashed = HashGenerator.Hash(refreshToken);
        return await _uow.UserJwtTokenRepo.GetAll()
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(X => X.RefreshTokenHash == refresheTokenHashed, cancellationToken);
    }
}
