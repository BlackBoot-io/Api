﻿#nullable disable
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlackBoot.Services.Implementations;

public abstract class JwtTokensFactory
{
    private readonly JwtSettings _jwtSettings;
    protected JwtTokensFactory(IConfiguration configuration) => _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

    protected IActionResponse<(string Token, int TokenExpirationMinutes)> CreateToken(List<Claim> claims, JwtTokenType tokenType)
    {
        var expirationTimeMinutes = tokenType switch
        {
            JwtTokenType.AccessToken => _jwtSettings.AccessTokenExpirationMinutes,
            JwtTokenType.RefreshToken => _jwtSettings.RefreshTokenExpirationMinutes,
            _ => throw new ArgumentOutOfRangeException(nameof(tokenType), $"Not expected tokenType value: {tokenType}"),
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var secretKey = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var issuerSigningKey = new SymmetricSecurityKey(secretKey);

        var encryptionkey = Encoding.UTF8.GetBytes(_jwtSettings.EncryptionKey);
        var tokenDecryptionKey = new SymmetricSecurityKey(encryptionkey);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(expirationTimeMinutes),
            SigningCredentials = new SigningCredentials(issuerSigningKey, SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwtSettings.Issuer,
            Audience = _jwtSettings.Audience,
            EncryptingCredentials = new EncryptingCredentials(tokenDecryptionKey,
                                                            SecurityAlgorithms.Aes256KW,
                                                            SecurityAlgorithms.Aes256CbcHmacSha512)
        };
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return new ActionResponse<(string Token, int TokenExpirationMinutes)>((tokenHandler.WriteToken(token), expirationTimeMinutes));
    }
    protected IActionResponse<ClaimsPrincipal> ReadToken(string token)
    {
        var secretKey = Encoding.UTF8.GetBytes(_jwtSettings.Key);
        var issuerSigningKey = new SymmetricSecurityKey(secretKey);

        var encryptionkey = Encoding.UTF8.GetBytes(_jwtSettings.EncryptionKey);
        var tokenDecryptionKey = new SymmetricSecurityKey(encryptionkey);

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
            ClockSkew = TimeSpan.Zero,
            ValidateIssuerSigningKey = true,
            TokenDecryptionKey = tokenDecryptionKey,
            IssuerSigningKey = issuerSigningKey
        };

        var principal = new JwtSecurityTokenHandler().ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
        if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.Aes256KW, StringComparison.InvariantCultureIgnoreCase))
            return new ActionResponse<ClaimsPrincipal>(ActionResponseStatusCode.NotFound);

        return new ActionResponse<ClaimsPrincipal>(principal);
    }
}


/// <summary>
/// Internal Api
/// </summary>
public class JwtTokensService : JwtTokensFactory, IJwtTokensService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IAppUnitOfWork _uow;
    private readonly IUsersService _usersService;
    private readonly ICacheService _cacheService;

    public JwtTokensService(IAppUnitOfWork uow,
                            IConfiguration configuration,
                            IUsersService usersService,
                            ICacheService cacheService) : base(configuration)
    {
        _uow = uow;
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        _usersService = usersService;
        _cacheService = cacheService;
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
        var accessToken = CreateToken(new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,userId.ToString()),
                new Claim(ClaimTypes.Email,email)
            }, JwtTokenType.AccessToken);

        var refreshToken = CreateToken(new List<Claim>
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
        var tokens = await _uow.UserJwtTokenRepo
                                                .Where(t => t.RefreshTokenHash == hashedRefreshToken).ToListAsync(cancellationToken);
        _uow.UserJwtTokenRepo.RemoveRange(tokens);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    private async Task<bool> CheckTryCount(Guid userId, CancellationToken cancellationToken = default)
    {
        var tryCount = await _cacheService.GetAsync<int>("TryCount" + userId.ToString(), cancellationToken);
        if (tryCount is > 4)
        {
            await _usersService.LockAsync(userId, DateTime.UtcNow.AddDays(1));
            return true;

        }
        else
            await _cacheService.SetAsync("TryCount" + userId.ToString(), tryCount + 1, TimeSpan.FromHours(4), cancellationToken);

        return false;
    }
    #endregion

    ///  <inheritdoc/>
    public async Task<IActionResponse<UserTokenDto>> GenerateUserTokenAsync(User user, string refreshToken = "", CancellationToken cancellationToken = default)
    {
        if (!string.IsNullOrEmpty(refreshToken))
            await DeleteTokensWithSameRefreshTokenAsync(refreshToken, cancellationToken);


        var tryCount = await CheckTryCount(user.UserId, cancellationToken);
        if (tryCount)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.ServerError, "");

        var token = GenerateToken(user.UserId, user.Email);
        _uow.UserJwtTokenRepo.Add(new UserJwtToken
        {
            UserId = user.UserId,
            AccessTokenHash = HashGenerator.Hash(token.AccessToken),
            AccessTokenExpiresTime = DateTime.Now.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            RefreshTokenHash = HashGenerator.Hash(token.RefreshToken),
            RefreshTokenExpiresTime = DateTime.Now.AddMinutes(_jwtSettings.RefreshTokenExpirationMinutes)
        });

        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse<UserTokenDto>(ActionResponseStatusCode.ServerError, dbResult.Message);

        token.User = new UserDto
        {
            Email = user.Email,
            EmailIsApproved = user.EmailIsApproved,
            FullName = user.FullName,
            OrganizationName = user.OrganizationName,
            Type = user.Type.ToString(),
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
            return new ActionResponse(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidPrameter);

        var hashedRefreshToken = HashGenerator.Hash(refreshToken);
        var tokens = await _uow.UserJwtTokenRepo
                                                .Where(t => t.RefreshTokenHash == hashedRefreshToken).ToListAsync(cancellationToken);
        _uow.UserJwtTokenRepo.RemoveRange(tokens);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.IsSuccess)
            return new ActionResponse(ActionResponseStatusCode.ServerError, dbResult.Message);

        await _cacheService.RemoveAsync("TryCount" + tokens.FirstOrDefault()?.UserId.ToString() ?? "");

        return new ActionResponse();
    }

    /// <summary>
    /// get jwt record from refresh token
    /// </summary>
    /// <param name="refreshToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<UserJwtToken> GetRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var refresheTokenHashed = HashGenerator.Hash(refreshToken);
        return await _uow.UserJwtTokenRepo
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(x => x.RefreshTokenHash == refresheTokenHashed, cancellationToken);
    }
    /// <summary>
    /// Check If User Own This Token or Not
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="accessToken"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IActionResponse<bool>> VerifyTokenAsync(Guid userId, string accessToken, CancellationToken cancellationToken = default)
    {
        var hashesAccessToken = HashGenerator.Hash(accessToken);
        var token = await _uow.UserJwtTokenRepo.AsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId && !x.User.IsLockoutEnabled && x.AccessTokenHash == hashesAccessToken, cancellationToken);
        return new ActionResponse<bool>(token is not null && token.AccessTokenExpiresTime >= DateTime.Now);
    }
}