using Avn.Domain.Enums;
using System.Security.Claims;

namespace Avn.Services.Interfaces;

public interface IJwtTokensFactory : IScopedDependency
{
    IActionResponse<(string Token, int TokenExpirationMinutes)> CreateToken(List<Claim> claims, JwtTokenType tokenType);
    IActionResponse<ClaimsPrincipal> ReadToken(string token);
}