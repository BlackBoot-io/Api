#nullable disable
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

    public JwtTokensService(IAppUnitOfWork uow, IConfiguration configuration) : base(configuration)
    {
        _uow = uow;
        _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
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
            return new ActionResponse(ActionResponseStatusCode.BadRequest, BusinessMessage.InvalidPrameter);

        var hashedRefreshToken = HashGenerator.Hash(refreshToken);
        var tokens = await _uow.UserJwtTokenRepo.GetAll()
                                                .Where(t => t.RefreshTokenHash == hashedRefreshToken).ToListAsync(cancellationToken);
        _uow.UserJwtTokenRepo.RemoveRange(tokens);
        var dbResult = await _uow.SaveChangesAsync(cancellationToken);
        if (!dbResult.ToSaveChangeResult())
            return new ActionResponse(ActionResponseStatusCode.ServerError);

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
        return await _uow.UserJwtTokenRepo.GetAll()
                                          .AsNoTracking()
                                          .FirstOrDefaultAsync(X => X.RefreshTokenHash == refresheTokenHashed, cancellationToken);
    }
}