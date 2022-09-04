using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlackBoot.Services.Implementations;

public class JwtTokensFactory : IJwtTokensFactory
{
    private readonly JwtSettings _jwtSettings;
    public JwtTokensFactory(IConfiguration configuration) => _jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();

    public IActionResponse<(string Token, int TokenExpirationMinutes)> CreateToken(List<Claim> claims, JwtTokenType tokenType)
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
    public IActionResponse<ClaimsPrincipal> ReadToken(string token)
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